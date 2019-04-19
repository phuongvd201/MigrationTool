using System;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Entities
{
    internal class MigrationContext
    {
        public Guid MigrationId { get; set; }

        public MigrationSourceSystem SourceSystem { get; set; }

        public string DataSource { get; set; }

        public bool Parallel { get; set; }
    }
}