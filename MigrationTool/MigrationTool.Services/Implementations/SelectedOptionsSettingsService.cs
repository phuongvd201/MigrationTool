using System;
using System.Configuration;
using System.Globalization;
using System.Linq;

using MigrationTool.Services.Interfaces;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations
{
    internal class SelectedOptionsSettingsService : SettingsServiceBase, ISelectedOptionsSettingsService
    {
        private const string SelectedMigrationEntitiesKey = "SelectedMigrationEntities";

        private const string SelectedMigrationDateTimeKey = "SelectedMigrationDateTime";

        private const string SelectedMigrationSourceSystemKey = "SelectedMigrationSourceSystem";

        private const string SelectedMigrationSourceNameKey = "SelectedMigrationSourceName";

        private const string SelectedParallelReadOptionKey = "SelectedParallelReadOption";

        public MigrationEntity[] SelectedMigrationEntities
        {
            get
            {
                var readSetting = ConfigurationManager.AppSettings[SelectedMigrationEntitiesKey];

                var result = string.IsNullOrWhiteSpace(readSetting)
                    ? new MigrationEntity[] { }
                    : readSetting.Split(',').Select(x =>
                    {
                        MigrationEntity parsed;
                        return Enum.TryParse(x, out parsed) ? parsed : 0;
                    }).Where(x => x != 0).ToArray();

                return result;
            }

            set { SaveInConfigFile(SelectedMigrationEntitiesKey, string.Join(",", value)); }
        }

        public MigrationSourceSystem SelectedMigrationSourceSystem
        {
            get
            {
                MigrationSourceSystem result;
                return Enum.TryParse(ConfigurationManager.AppSettings[SelectedMigrationSourceSystemKey], out result)
                    ? result
                    : 0;
            }

            set { SaveInConfigFile(SelectedMigrationSourceSystemKey, value.ToString()); }
        }

        public DateTime SelectedMigrationDateTime
        {
            get
            {
                DateTime result;
                return DateTime.TryParse(ConfigurationManager.AppSettings[SelectedMigrationDateTimeKey], out result)
                    ? result
                    : DateTime.MinValue;
            }

            set { SaveInConfigFile(SelectedMigrationDateTimeKey, value.ToString(CultureInfo.InvariantCulture)); }
        }

        public bool SelectedParallelReadOption
        {
            get { return ConfigurationManager.AppSettings[SelectedParallelReadOptionKey] == "True"; }

            set { SaveInConfigFile(SelectedParallelReadOptionKey, value.ToString()); }
        }

        public string SelectedMigrationSourceName
        {
            get { return ConfigurationManager.AppSettings[SelectedMigrationSourceNameKey]; }

            set { SaveInConfigFile(SelectedMigrationSourceNameKey, value); }
        }
    }
}