using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Interfaces;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;
using Siberia.Migration.Serialization.Data;
using Siberia.Migration.Serialization.Json;

using Log4net = log4net;

namespace MigrationTool.Services.Helpers
{
    internal static class MigrationHelper
    {
        private static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static EntityMigrationContext PrepareAndStoreBsonFiles(
            this IEnumerable<IMigrationEntity> source,
            MigrationEntity entityType,
            MigrationContext migrationContext,
            IAppSettingsService appSettings,
            ISupportedEntitiesInfoProvider supportService,
            IMigrationStatusService statusService,
            IPostProcessingService postProcessingService,
            IEntitiesMemoryService memoryService,
            ConcurrentQueue<MigrationFileContext> filesQueue,
            Action<MigrationEntity, MigrationStage> reportProgress,
            ConcurrentBag<Exception> errors)
        {
            var entityContext = new EntityMigrationContext
            {
                EntityType = entityType
            };

            try
            {
                MergeStatuses(migrationContext, entityType, statusService);

                source
                    .ToPartitionedEnumerable(
                        migrationContext.Parallel
                        & (memoryService.IsMeasurable(migrationContext.SourceSystem, entityType)
                           || supportService.PreferredBatchSizes.ContainsKey(entityType))
                            ? appSettings.ParallelBatchesNumber
                            : 1)
                    .ForEach(
                        true,
                        partition =>
                        {
                            var preparedData = partition
                                .PostProcess(
                                    migrationContext,
                                    entityContext,
                                    postProcessingService)
                                .FilterWithStatuses(
                                    migrationContext,
                                    entityContext,
                                    statusService,
                                    reportProgress);

                            preparedData.ProcessInBatches(
                                entityContext,
                                migrationContext,
                                appSettings,
                                supportService,
                                memoryService,
                                batch =>
                                {
                                    var batchId = Guid.NewGuid();
                                    Log.InfoFormat("Batch {0} -> {1} -> Started.", batchId, entityType);
                                    var fileName = string.Format(
                                        "{0}-{1}-{2}.json",
                                        migrationContext.MigrationId,
                                        entityContext.EntityType,
                                        batchId);
                                    var fileContext = new MigrationFileContext
                                    {
                                        BatchId = batchId,
                                        FileName = fileName,
                                        EntityType = entityContext.EntityType,
                                        MigrationId = migrationContext.MigrationId
                                    };

                                    C2cJson.Write(
                                        batch
                                            .ReportProcess(
                                                entityContext,
                                                reportProgress)
                                            .PopulateStatuses(
                                                migrationContext,
                                                entityContext,
                                                fileContext,
                                                statusService))
                                        .ToBson()
                                        .ToFile(fileName);

                                    reportProgress(entityContext.EntityType, MigrationStage.BatchPrepared);
                                    filesQueue.Enqueue(fileContext);
                                    Log.InfoFormat("Batch {0} -> {1} -> Prepared.", batchId, entityType);
                                });
                        });
            }
            catch (Exception e)
            {
                Log.Error(e);
                reportProgress(entityContext.EntityType, MigrationStage.Error);
                errors.Add(e);
                return entityContext;
            }

            Log.InfoFormat("{0} -> All prepared.", entityType);
            reportProgress(entityContext.EntityType, MigrationStage.PreparationReady);
            return entityContext;
        }

        public static void SendFiles(
            CancellationToken ct,
            MigrationContext migrationContext,
            IAuthenticationService authService,
            IMigrationStatusService statusService,
            IAppSettingsService appSettings,
            ISupportedEntitiesInfoProvider supportService,
            ConcurrentQueue<MigrationFileContext> queue,
            Action<MigrationEntity, MigrationStage> reportProgress,
            ConcurrentBag<Exception> errors)
        {
            while (true)
            {
                MigrationFileContext context;

                if (queue.TryDequeue(out context))
                {
                    try
                    {
                        Log.InfoFormat("Batch {0} -> {1} -> Sending.", context.BatchId, context.EntityType);
                        StageStatuses(migrationContext, context, statusService);

                        StorageHelper
                            .UseStream(
                                write =>
                                {
                                    var exception = write.ToMigrationFileServer(
                                        appSettings.SiberiaBaseUrl,
                                        appSettings.SiberiaUploadFileUrl,
                                        authService.GetCookieName,
                                        authService.GetAuth,
                                        context.MigrationId,
                                        context.EntityType,
                                        context.FileName,
                                        appSettings.RetryCount);
                                    if (exception != null)
                                    {
                                        throw exception;
                                    }
                                })
                            .FromFile(context.FileName);
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error when sending batch file " + context.BatchId + ".", e);
                        reportProgress(context.EntityType, MigrationStage.Error);
                        errors.Add(e);
                        continue;
                    }
                    finally
                    {
                        CommitStatuses(migrationContext, context, statusService);
                        File.Delete(context.FileName);
                    }

                    MergeStatuses(migrationContext, context.EntityType, statusService);

                    Log.InfoFormat("Batch {0} -> {1} -> Sent.", context.BatchId, context.EntityType);
                    reportProgress(context.EntityType, MigrationStage.BatchProcessed);

                    continue;
                }

                Thread.Sleep(200);
                if (ct.IsCancellationRequested && queue.IsEmpty)
                {
                    Log.InfoFormat("All sent.");
                    break;
                }
            }
        }

        private static void ProcessInBatches(
            this IEnumerable<IMigrationEntity> source,
            EntityMigrationContext entityContext,
            MigrationContext migrationContext,
            IAppSettingsService appSettings,
            ISupportedEntitiesInfoProvider supportService,
            IEntitiesMemoryService memoryService,
            Action<IEnumerable<IMigrationEntity>> processBatch)
        {
            if (memoryService.IsMeasurable(migrationContext.SourceSystem, entityContext.EntityType))
            {
                var getMemory = memoryService.GetMemoryMeasurementFunction(migrationContext.SourceSystem, entityContext.EntityType);

                source
                    .ToBatchedEnumerable(
                        appSettings.PreferredBatchSizeInBytes,
                        getMemory)
                    .ForEach(false, processBatch);
                return;
            }

            if (supportService.PreferredBatchSizes.ContainsKey(entityContext.EntityType))
            {
                source
                    .ToPagedEnumerable(
                        supportService.PreferredBatchSizes[entityContext.EntityType])
                    .ForEach(false, processBatch);
                return;
            }

            processBatch(source);
        }

        private static IEnumerable<IMigrationEntity> FilterWithStatuses(
            this IEnumerable<IMigrationEntity> source,
            MigrationContext migrationContext,
            EntityMigrationContext entityContext,
            IMigrationStatusService statusService,
            Action<MigrationEntity, MigrationStage> reportProgress
            )
        {
            if (!statusService.IsSupported(migrationContext.SourceSystem, entityContext.EntityType))
            {
                return source;
            }
            var extractStatus = statusService.GetStatusExtractionMethod(migrationContext.SourceSystem, entityContext.EntityType);
            var statuses = statusService.Get(migrationContext.DataSource, entityContext.EntityType);

            return source
                .Where(x =>
                {
                    var status = extractStatus(x);
                    var letPass = status.NeedToSend(statuses);
                    if (letPass)
                    {
                        return true;
                    }
                    if (status.Status == MigrationStatus.Error)
                    {
                        Log.Warn(status.Text);
                    }
                    reportProgress(entityContext.EntityType, MigrationStage.Skipping);
                    return false;
                });
        }

        private static IEnumerable<IMigrationEntity> PopulateStatuses(
            this IEnumerable<IMigrationEntity> source,
            MigrationContext migrationContext,
            EntityMigrationContext entityContext,
            MigrationFileContext fileContext,
            IMigrationStatusService statusService
            )
        {
            if (!statusService.IsSupported(migrationContext.SourceSystem, entityContext.EntityType))
            {
                return source;
            }
            var extractStatus = statusService.GetStatusExtractionMethod(migrationContext.SourceSystem, entityContext.EntityType);
            fileContext.Statuses = new ConcurrentDictionary<string, EntityMigrationStatus>();

            return source
                .Select(x =>
                {
                    var status = extractStatus(x);
                    fileContext.Statuses[status.Key] = status;
                    return x;
                });
        }

        private static void StageStatuses(
            MigrationContext migrationContext,
            MigrationFileContext fileContext,
            IMigrationStatusService statusService
            )
        {
            if (!statusService.IsSupported(migrationContext.SourceSystem, fileContext.EntityType))
            {
                return;
            }

            statusService.Stage(migrationContext.DataSource, fileContext.EntityType, fileContext.BatchId, fileContext.Statuses);
        }

        private static void CommitStatuses(
            MigrationContext migrationContext,
            MigrationFileContext fileContext,
            IMigrationStatusService statusService,
            Exception e = null
            )
        {
            if (!statusService.IsSupported(migrationContext.SourceSystem, fileContext.EntityType))
            {
                return;
            }

            if (e == null)
            {
                statusService.Commit(fileContext.BatchId);
                return;
            }

            statusService.Commit(fileContext.BatchId, e);
        }

        private static void MergeStatuses(
            MigrationContext migrationContext,
            MigrationEntity entityType,
            IMigrationStatusService statusService
            )
        {
            if (!statusService.IsSupported(migrationContext.SourceSystem, entityType))
            {
                return;
            }

            statusService.Merge(migrationContext.DataSource, entityType);
        }

        private static IEnumerable<IMigrationEntity> PostProcess(
            this IEnumerable<IMigrationEntity> source,
            MigrationContext migrationContext,
            EntityMigrationContext entityContext,
            IPostProcessingService postProcessingService
            )
        {
            if (!postProcessingService.IsSupported(migrationContext.SourceSystem, entityContext.EntityType))
            {
                return source;
            }
            var postProcess = postProcessingService.GetPostProcessingMethod(migrationContext.SourceSystem, entityContext.EntityType);

            return source.Select(postProcess).Where(x => x != null);
        }

        private static IEnumerable<IMigrationEntity> ReportProcess(
            this IEnumerable<IMigrationEntity> source,
            EntityMigrationContext entityContext,
            Action<MigrationEntity, MigrationStage> reportProgress)
        {
            return source.Select(x =>
            {
                reportProgress(entityContext.EntityType, MigrationStage.Preparation);
                return x;
            });
        }

        private static bool NeedToSend(this EntityMigrationStatus newStatus, ConcurrentDictionary<string, EntityMigrationStatus> storedStatuses)
        {
            return newStatus.Status == MigrationStatus.Ok &&
                   (!storedStatuses.ContainsKey(newStatus.Key)
                    || (storedStatuses[newStatus.Key].Status == MigrationStatus.Ok && newStatus.Text != storedStatuses[newStatus.Key].Text)
                    || storedStatuses[newStatus.Key].Status != MigrationStatus.Ok);
        }
    }
}