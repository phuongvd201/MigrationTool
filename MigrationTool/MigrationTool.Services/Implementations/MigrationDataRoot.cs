using System;
using System.Collections.Generic;
using System.Linq;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Interfaces;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations
{
    internal class MigrationDataRoot : IMigrationDataProcessor, ISupportedEntitiesInfoProvider
    {
        public IEnumerable<IEntitiesContainer> DataSourceContainers { get; set; }

        public MigrationSourceSystem[] SupportedSourceSystems
        {
            get
            {
                return DataSourceContainers
                    .Select(x => x.SourceSystem)
                    .OrderBy(x => (int)x).ToArray();
            }
        }

        private MigrationSourceSystem[] localEntityTypeParallelismSupportingSystems;

        public MigrationSourceSystem[] EntityTypeParallelismSupportingSystems
        {
            get
            {
                if (localEntityTypeParallelismSupportingSystems == null)
                {
                    localEntityTypeParallelismSupportingSystems = new[]
                    {
                        MigrationSourceSystem.C2cXml,
                    };
                }
                return localEntityTypeParallelismSupportingSystems;
            }
        }

        private Dictionary<MigrationEntity, int> localPreferredBatchSizes;

        public Dictionary<MigrationEntity, int> PreferredBatchSizes
        {
            get
            {
                if (localPreferredBatchSizes == null)
                {
                    localPreferredBatchSizes = new Dictionary<MigrationEntity, int>
                    {
                        { MigrationEntity.Letters, 2000 },
                        { MigrationEntity.LaboratoryResults, 1000 },
                        { MigrationEntity.Documents, 500 },
                    };
                }
                return localPreferredBatchSizes;
            }
        }

        public void ProcessEntities(Action<IEnumerable<IMigrationEntity>> processEntities, MigrationSourceSystem sourceSystem, MigrationEntity entityType, MigrationArgs args)
        {
            DataSourceContainers
                .First(x => x.SourceSystem == sourceSystem)
                .ProcessEntities(processEntities, entityType, args);
        }

        public MigrationEntity[] GetSupportedEntityTypes(MigrationSourceSystem sourceSystem)
        {
            return DataSourceContainers
                .First(x => x.SourceSystem == sourceSystem)
                .SupportedEntityTypes
                .OrderBy(x => (int)x).ToArray();
        }
    }
}