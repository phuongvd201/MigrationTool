using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Entities
{
    internal class EntityMigrationStatus : IMigrationEntity
    {
        public string Key { get; set; }

        public MigrationStatus Status { get; set; }

        public string Text { get; set; }
    }
}