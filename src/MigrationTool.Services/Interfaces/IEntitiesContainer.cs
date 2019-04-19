using System;
using System.Collections.Generic;

using MigrationTool.Services.Entities;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    internal interface IEntitiesContainer
    {
        MigrationEntity[] SupportedEntityTypes { get; }

        MigrationSourceSystem SourceSystem { get; }

        void ProcessEntities(Action<IEnumerable<IMigrationEntity>> processEntities, MigrationEntity entityType, MigrationArgs args);
    }
}