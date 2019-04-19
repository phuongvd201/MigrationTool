using System;
using System.Collections.Concurrent;

using MigrationTool.Services.Entities;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    internal interface IMigrationStatusService
    {
        bool IsSupported(MigrationSourceSystem sourceSystem, MigrationEntity entityType);

        Func<IMigrationEntity, EntityMigrationStatus> GetStatusExtractionMethod(MigrationSourceSystem sourceSystem, MigrationEntity entityType);

        void Merge(string sourceName, MigrationEntity entityType);

        void Stage(string sourceName, MigrationEntity entityType, Guid batchId, ConcurrentDictionary<string, EntityMigrationStatus> statuses);

        void Commit(Guid batchId, Exception e);

        void Commit(Guid batchId);

        ConcurrentDictionary<string, EntityMigrationStatus> Get(string sourceName, MigrationEntity entityType);
    }
}