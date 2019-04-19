using System;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    internal interface IPostProcessingService
    {
        bool IsSupported(MigrationSourceSystem sourceSystem, MigrationEntity entityType);

        Func<IMigrationEntity, IMigrationEntity> GetPostProcessingMethod(MigrationSourceSystem sourceSystem, MigrationEntity entityType);
    }
}