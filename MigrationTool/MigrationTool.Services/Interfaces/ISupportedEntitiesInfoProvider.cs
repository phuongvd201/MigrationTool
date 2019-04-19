using System.Collections.Generic;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    public interface ISupportedEntitiesInfoProvider
    {
        MigrationSourceSystem[] SupportedSourceSystems { get; }

        Dictionary<MigrationEntity, int> PreferredBatchSizes { get; }

        MigrationSourceSystem[] EntityTypeParallelismSupportingSystems { get; }

        MigrationEntity[] GetSupportedEntityTypes(MigrationSourceSystem sourceSystem);
    }
}