using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using MigrationTool.Services.Interfaces.MedicalDirector;

namespace MigrationTool.Services.Implementations.MedicalDirector
{
    internal class MedicalDirectorSettingsService : SettingsServiceBase, IMedicalDirectorSettingsService
    {
        private const string MedicalDirectorDatabaseHCN = "HCN_SAMPLES";
        private const string MedicalDirectorDatabaseContent = "HCN_CONTENT";
        private const string MedicalDirectorDatabaseServerNameKey = "MedicalDirectorDatabaseServerName";
        private const string MedicalDirectorDocumentPathKey = "MedicalDirectorDocumentPath";

        public string DatabaseServerName
        {
            get { return ConfigurationManager.AppSettings[MedicalDirectorDatabaseServerNameKey]; }
            set { SaveInConfigFile(MedicalDirectorDatabaseServerNameKey, value); }
        }

        public string DocumentsPath
        {
            get { return ConfigurationManager.AppSettings[MedicalDirectorDocumentPathKey]; }
            set { SaveInConfigFile(MedicalDirectorDocumentPathKey, value); }
        }

        public string HcnConnectionString
        {
            get
            {
                var pairs = new[]
                {
                    new KeyValuePair<string, string>("Data Source", DatabaseServerName),
                    new KeyValuePair<string, string>("Initial Catalog", MedicalDirectorDatabaseHCN),
                    new KeyValuePair<string, string>("Integrated Security", "True"),
                };

                return string.Join(
                    ";",
                    pairs
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                        .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                    );
            }
        }

        public string ContentConnectionString
        {
            get
            {
                var pairs = new[]
                {
                    new KeyValuePair<string, string>("Data Source", DatabaseServerName),
                    new KeyValuePair<string, string>("Initial Catalog", MedicalDirectorDatabaseContent),
                    new KeyValuePair<string, string>("Integrated Security", "True"),
                };

                return string.Join(
                    ";",
                    pairs
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                        .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                    );
            }
        }
    }
}