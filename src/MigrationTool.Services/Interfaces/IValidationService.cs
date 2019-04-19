using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    public interface IValidationService
    {
        string[] ValidateSettings(MigrationSourceSystem sourceSystem, MigrationEntity[] selectedEntities);

        string[] ValidateSettings(MigrationSourceSystem sourceSystem);

        string[] ValidateDataSourceName(string dataSourceName);
    }
}