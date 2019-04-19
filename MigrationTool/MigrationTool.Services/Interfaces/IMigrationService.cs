using System;

using MigrationTool.Services.Entities;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    public interface IMigrationService
    {
        bool Migrate(Action<MigrationEntity, MigrationStage> reportProgress);
    }
}