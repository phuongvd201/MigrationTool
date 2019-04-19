using System;
using System.Collections.Concurrent;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Entities
{
    internal class MigrationFileContext
    {
        public string FileName { get; set; }

        public Guid MigrationId { get; set; }

        public MigrationEntity EntityType { get; set; }

        public Guid BatchId { get; set; }

        public ConcurrentDictionary<string, EntityMigrationStatus> Statuses { get; set; }
    }
}