using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces;

using RestSharp;

using Siberia.Migration.Entities.Common;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations
{
    internal class MigrationService : IMigrationService
    {
        private static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IAppSettingsService AppSettings { get; set; }

        public ISupportedEntitiesInfoProvider SupportService { get; set; }

        public IAuthenticationService AuthenticationService { get; set; }

        public IMigrationStatusService MigrationStatusService { get; set; }

        public IPostProcessingService PostProcessingService { get; set; }

        public IEntitiesMemoryService MemoryService { get; set; }

        public IMigrationDataProcessor MigrationDataProcessor { get; set; }

        public ISelectedOptionsSettingsService SelectedOptions { get; set; }

        public bool Migrate(Action<MigrationEntity, MigrationStage> reportProgress)
        {
            var start = SelectedOptions.SelectedMigrationDateTime;
            var sourceSystem = SelectedOptions.SelectedMigrationSourceSystem;
            var sourceName = SelectedOptions.SelectedMigrationSourceName;
            var migrationGuid = RegisterMigration(sourceSystem, sourceName, start);

            if (!migrationGuid.HasValue)
            {
                return false;
            }

            var entityTypes = SelectedOptions.SelectedMigrationEntities;
            var parallel = SelectedOptions.SelectedParallelReadOption;

            var args = new MigrationArgs
            {
                Start = start,
            };

            var migrationContext = new MigrationContext
            {
                DataSource = sourceName,
                MigrationId = migrationGuid.Value,
                SourceSystem = sourceSystem,
                Parallel = parallel
            };

            var errors = new ConcurrentBag<Exception>();
            var filesQueue = new ConcurrentQueue<MigrationFileContext>();
            var cts = new CancellationTokenSource();

            Parallel.Invoke(
                () =>
                {
                    entityTypes.ForEach(
                        SupportService.EntityTypeParallelismSupportingSystems.Contains(sourceSystem),
                        entityType =>
                        {
                            var localEntityType = entityType;

                            MigrationDataProcessor.ProcessEntities(
                                entities =>
                                {
                                    entities.PrepareAndStoreBsonFiles(
                                        entityType,
                                        migrationContext,
                                        AppSettings,
                                        SupportService,
                                        MigrationStatusService,
                                        PostProcessingService,
                                        MemoryService,
                                        filesQueue,
                                        reportProgress,
                                        errors);
                                },
                                sourceSystem,
                                localEntityType,
                                args);
                        });
                    Log.InfoFormat("All prepared.");
                    cts.Cancel();
                },
                () => MigrationHelper.SendFiles(
                    cts.Token,
                    migrationContext,
                    AuthenticationService,
                    MigrationStatusService,
                    AppSettings,
                    SupportService,
                    filesQueue,
                    reportProgress,
                    errors));

            return CommitMigration(migrationContext.MigrationId) && !errors.Any();
        }

        private Guid? RegisterMigration(MigrationSourceSystem sourceSystem, string sourceName, DateTime? start)
        {
            Log.Info("Registering migration");

            var client = new RestClient(AppSettings.SiberiaBaseUrl);
            var request = new RestRequest(AppSettings.SiberiaRegisterMigrationUrl, Method.POST);

            request.AddCookie(AuthenticationService.GetCookieName(), AuthenticationService.GetAuth());

            request.AddParameter("Version", AppSettings.Version);
            request.AddParameter("MigrationSourceName", sourceName);
            request.AddParameter("MigrationSourceSystem", (int)sourceSystem);

            if (start.HasValue)
            {
                request.AddParameter("MigrationStart", start);
            }

            var response = client.Execute(request);
            return response.InterpretResponse<Guid?>(Log, "register migration");
        }

        private bool CommitMigration(Guid migrationId)
        {
            Log.Info("Committing migration");

            var client = new RestClient(AppSettings.SiberiaBaseUrl);
            var request = new RestRequest(AppSettings.SiberiaCommitMigrationUrl, Method.POST);

            request.AddCookie(AuthenticationService.GetCookieName(), AuthenticationService.GetAuth());

            request.AddParameter("Id", migrationId);

            var response = client.Execute(request);
            return response.InterpretResponse<bool>(Log, "commit migration");
        }
    }
}