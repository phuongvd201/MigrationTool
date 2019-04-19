using System.Configuration;

namespace MigrationTool.Services.Implementations
{
    internal abstract class SettingsServiceBase
    {
        private Configuration ConfigFile
        {
            get { return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); }
        }

        protected void SaveInConfigFile(string key, string value)
        {
            var localConfig = ConfigFile;

            var settings = localConfig.AppSettings.Settings;
            if (settings[key] == null)
            {
                settings.Add(key, value);
            }
            else
            {
                settings[key].Value = value;
            }

            localConfig.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(localConfig.AppSettings.SectionInformation.Name);
        }
    }
}