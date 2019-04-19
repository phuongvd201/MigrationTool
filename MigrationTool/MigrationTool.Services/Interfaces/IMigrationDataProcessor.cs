using System;
using System.Collections.Generic;

using MigrationTool.Services.Entities;

using Siberia.Migration.Entities;
using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    internal interface IMigrationDataProcessor
    {
        void ProcessEntities(Action<IEnumerable<IMigrationEntity>> processEntities, MigrationSourceSystem sourceSystem, MigrationEntity entityType, MigrationArgs args);
    }
}