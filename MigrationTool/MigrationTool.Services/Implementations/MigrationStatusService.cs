using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;
using Siberia.Migration.Serialization.Json;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations
{
    internal class MigrationStatusService : IMigrationStatusService
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string FileExtension = ".dat";

        private const string FileNamePartSeparator = "_";

        private const char UnsupportedFileNameBase64Char = '/';

        private const char ReplacementFileNameBase64Char = '-';

        private readonly string localPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        private Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, EntityMigrationStatus>> localGetStatusFunctions;

        private Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, EntityMigrationStatus>> SupportedGetStatusFunctions
        {
            get
            {
                if (localGetStatusFunctions == null)
                {
                    localGetStatusFunctions = new Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, EntityMigrationStatus>>
                    {
                        {
                            Tuple.Create(MigrationSourceSystem.C2cXml, MigrationEntity.Documents),
                            GetStatusFromDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Genie, MigrationEntity.Documents),
                            GetStatusFromDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Shexie, MigrationEntity.Documents),
                            GetStatusFromDocument
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Shexie, MigrationEntity.LaboratoryResults),
                            GetStatusFromLabResult
                        },
                        {
                            Tuple.Create(MigrationSourceSystem.Shexie, MigrationEntity.Letters),
                            GetStatusFromLetter
                        },
                    };
                }
                return localGetStatusFunctions;
            }
        }

        public bool IsSupported(MigrationSourceSystem source, MigrationEntity entityType)
        {
            return SupportedGetStatusFunctions.ContainsKey(Tuple.Create(source, entityType));
        }

        public Func<IMigrationEntity, EntityMigrationStatus> GetStatusExtractionMethod(MigrationSourceSystem sourceSystem, MigrationEntity entityType)
        {
            return SupportedGetStatusFunctions[Tuple.Create(sourceSystem, entityType)];
        }

        public void Merge(string sourceName, MigrationEntity entityType)
        {
            var baseFileName = ToHashedSourceName(sourceName)
                               + FileNamePartSeparator
                               + entityType;
            var mergedFileName = baseFileName + FileExtension;

            var mergedStatuses = Get(sourceName, entityType);

            var tempFileNames = Directory.GetFiles(localPath, baseFileName + FileNamePartSeparator + "*");

            var toDelete = new List<string>();

            try
            {
                foreach (var fileName in tempFileNames)
                {
                    toDelete.Add(fileName);
                    C2cJson.ReadAndProcess<EntityMigrationStatus>(statuses =>
                    {
                        statuses.ToList().ForEach(x =>
                        {
                            mergedStatuses[x.Key] = x;
                        });
                    }).FromJson().FromFile(fileName);
                }

                C2cJson.Write(mergedStatuses.Values).ToJson().ToFile(mergedFileName);

                foreach (var fileName in toDelete)
                {
                    File.Delete(fileName);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void Stage(string sourceName, MigrationEntity entityType, Guid batchId, ConcurrentDictionary<string, EntityMigrationStatus> statuses)
        {
            var fileName = ToHashedSourceName(sourceName)
                           + FileNamePartSeparator
                           + entityType
                           + FileNamePartSeparator
                           + batchId
                           + FileExtension;
            C2cJson.Write(statuses.Values).ToJson().ToFile(fileName);
        }

        public void Commit(Guid batchId)
        {
            Commit(
                batchId,
                x =>
                {
                    x.Status = MigrationStatus.Ok;
                    return x;
                });
        }

        public void Commit(Guid batchId, Exception e)
        {
            Commit(
                batchId,
                x =>
                {
                    x.Status = MigrationStatus.Error;
                    x.Text = e.Message;
                    return x;
                });
        }

        private void Commit(
            Guid batchId,
            Func<EntityMigrationStatus, EntityMigrationStatus> transform)
        {
            try
            {
                var batchFileNames = Directory.GetFiles(localPath, "*" + FileNamePartSeparator + batchId + "*.dat");

                foreach (var fileName in batchFileNames)
                {
                    C2cJson.ReadAndProcess<EntityMigrationStatus>(statuses =>
                    {
                        C2cJson.Write(statuses.Select(transform));
                    }).FromJson().FromFile(fileName);
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public ConcurrentDictionary<string, EntityMigrationStatus> Get(string sourceName, MigrationEntity entityType)
        {
            var mergedFileName = ToHashedSourceName(sourceName)
                                 + FileNamePartSeparator
                                 + entityType
                                 + FileExtension;

            var mergedStatuses = new ConcurrentDictionary<string, EntityMigrationStatus>();

            try
            {
                if (!File.Exists(mergedFileName))
                {
                    return mergedStatuses;
                }

                C2cJson.ReadAndProcess<EntityMigrationStatus>(statuses =>
                {
                    statuses.ForEach(true, x => mergedStatuses[x.Key] = x);
                }).FromJson().FromFile(mergedFileName);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return mergedStatuses;
        }

        private string ToHashedSourceName(string sourceName)
        {
            return string.IsNullOrWhiteSpace(sourceName)
                ? string.Empty
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(sourceName))
                    .Replace(UnsupportedFileNameBase64Char, ReplacementFileNameBase64Char);
        }

        private EntityMigrationStatus GetStatusFromDocument(IMigrationEntity source)
        {
            var document = (MigrationDocument)source;
            return new EntityMigrationStatus
            {
                Key = document.FileName,
                Status = document.FileData != null ? MigrationStatus.Ok : MigrationStatus.Error,
                Text = document.FileData != null ? document.Md5 : "File not found.",
            };
        }

        private EntityMigrationStatus GetStatusFromLabResult(IMigrationEntity source)
        {
            var result = (MigrationLaboratoryResult)source;

            return new EntityMigrationStatus
            {
                Key = result.ExternalId,
                Status = result.FormattedResult != null ? MigrationStatus.Ok : MigrationStatus.Error,
                Text = result.FormattedResult != null ? Encoding.UTF8.GetBytes(result.FormattedResult).Md5() : "Result not found.",
            };
        }

        private EntityMigrationStatus GetStatusFromLetter(IMigrationEntity source)
        {
            var letter = (MigrationLetter)source;

            return new EntityMigrationStatus
            {
                Key = letter.ExternalId,
                Status = letter.Text != null ? MigrationStatus.Ok : MigrationStatus.Error,
                Text = letter.Text != null ? Encoding.UTF8.GetBytes(letter.Text).Md5() : "Letter empty",
            };
        }
    }
}