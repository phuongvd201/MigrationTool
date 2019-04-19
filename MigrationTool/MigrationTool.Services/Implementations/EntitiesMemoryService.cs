using System;
using System.Collections.Generic;

using MigrationTool.Services.Interfaces;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations
{
    internal class EntitiesMemoryService : IEntitiesMemoryService
    {
        private Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, int>> localGetMemoryFunctions;

        private Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, int>> GetMemoryFunctions
        {
            get
            {
                if (localGetMemoryFunctions == null)
                {
                    localGetMemoryFunctions = new Dictionary<Tuple<MigrationSourceSystem, MigrationEntity>, Func<IMigrationEntity, int>>
                    {
                        {
                            Tuple.Create(MigrationSourceSystem.Genie, MigrationEntity.Documents),
                            GetDocumentSize
                        },
                    };
                }
                return localGetMemoryFunctions;
            }
        }

        public bool IsMeasurable(MigrationSourceSystem source, MigrationEntity entityType)
        {
            return GetMemoryFunctions.ContainsKey(Tuple.Create(source, entityType));
        }

        public Func<IMigrationEntity, int> GetMemoryMeasurementFunction(MigrationSourceSystem sourceSystem, MigrationEntity entityType)
        {
            return GetMemoryFunctions[Tuple.Create(sourceSystem, entityType)];
        }

        private int GetDocumentSize(IMigrationEntity source)
        {
            var document = (MigrationDocument)source;
            return document.FileData.Length;
        }
    }
}