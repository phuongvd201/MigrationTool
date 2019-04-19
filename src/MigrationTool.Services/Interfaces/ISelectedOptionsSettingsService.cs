using System;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Interfaces
{
    public interface ISelectedOptionsSettingsService
    {
        DateTime SelectedMigrationDateTime { get; set; }

        bool SelectedParallelReadOption { get; set; }

        MigrationEntity[] SelectedMigrationEntities { get; set; }

        MigrationSourceSystem SelectedMigrationSourceSystem { get; set; }

        string SelectedMigrationSourceName { get; set; }
    }
}