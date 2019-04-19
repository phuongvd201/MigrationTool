using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    public interface IMigrationSourceService
    {
        string[] GetPreviousSourceNames(MigrationSourceSystem sourceSystem);

        string CreateSource(MigrationSourceSystem sourceSystem, string sourceName);
    }
}