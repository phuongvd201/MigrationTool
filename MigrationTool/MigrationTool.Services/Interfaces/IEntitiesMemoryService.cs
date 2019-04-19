using System;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    internal interface IEntitiesMemoryService
    {
        bool IsMeasurable(MigrationSourceSystem source, MigrationEntity entityType);

        Func<IMigrationEntity, int> GetMemoryMeasurementFunction(MigrationSourceSystem sourceSystem, MigrationEntity entityType);
    }
}