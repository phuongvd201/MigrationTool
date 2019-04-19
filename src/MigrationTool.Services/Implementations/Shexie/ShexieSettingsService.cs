using System.Configuration;

using MigrationTool.Services.Interfaces.Shexie;

namespace MigrationTool.Services.Implementations.Shexie
{
    internal class ShexieSettingsService : SettingsServiceBase, IShexieSettingsService
    {
        private const string ShexieDatabaseFilePathKey = "ShexieDatabaseFilePath";
        private const string ShexieDocumentsPathKey = "ShexieDocumentsPath";

        public string DatabaseFilePath
        {
            get { return ConfigurationManager.AppSettings[ShexieDatabaseFilePathKey]; }

            set { SaveInConfigFile(ShexieDatabaseFilePathKey, value); }
        }

        public string DocumentsPath
        {
            get { return ConfigurationManager.AppSettings[ShexieDocumentsPathKey]; }

            set { SaveInConfigFile(ShexieDocumentsPathKey, value); }
        }

        public string ConnectionString
        {
            get { return @"Provider=Microsoft.ACE.OLEDB.12.0; data source=" + DatabaseFilePath; }
        }
    }
}