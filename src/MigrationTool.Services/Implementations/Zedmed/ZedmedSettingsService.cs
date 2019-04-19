using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

using MigrationTool.Services.Interfaces.Zedmed;

namespace MigrationTool.Services.Implementations.Zedmed
{
    internal class ZedmedSettingsService : SettingsServiceBase, IZedmedSettingsService
    {
        private const string ZedmedDatabaseSuperplusName = "SUPERPLUS";
        private const string ZedmedDatabaseClinplusName = "clinplus";
        private const string ZedmedUsernameKey = "ZedmedUsername";
        private const string ZedmedDatabaseFolderPathKey = "ZedmedDatabaseFolderPath";
        private const string ZedmedDocumentPathKey = "ZedmedDocumentPath";

        public string Username
        {
            get { return ConfigurationManager.AppSettings[ZedmedUsernameKey]; }
            set { SaveInConfigFile(ZedmedUsernameKey, value); }
        }

        public Func<string> Password { get; set; }

        public string DocumentsPath
        {
            get { return ConfigurationManager.AppSettings[ZedmedDocumentPathKey]; }
            set { SaveInConfigFile(ZedmedDocumentPathKey, value); }
        }

        public string DatabaseFolderPath
        {
            get { return ConfigurationManager.AppSettings[ZedmedDatabaseFolderPathKey]; }
            set { SaveInConfigFile(ZedmedDatabaseFolderPathKey, value); }
        }

        public string ClinplusConnectionString
        {
            get
            {
                var pairs = new[]
                {
                    new KeyValuePair<string, string>("User", Username),
                    new KeyValuePair<string, string>("Password", Password()),
                    new KeyValuePair<string, string>("Database", Path.Combine(DatabaseFolderPath, ZedmedDatabaseClinplusName)),
                };

                return string.Join(
                    ";",
                    pairs
                        .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                        .Select(x => string.Format("{0}={1}", x.Key, x.Value))
                    );
            }
        }

        public string SuperplusConnectionString
        {
            get
            {
                var pairs = new[]
                {
                    new KeyValuePair<string, string>("User", Username),
                    new KeyValuePair<string, string>("Password", Password()),
                    new KeyValuePair<string, string>("Database", Path.Combine(DatabaseFolderPath, ZedmedDatabaseSuperplusName)),
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