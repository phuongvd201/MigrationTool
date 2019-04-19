using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using MigrationTool.Services.Interfaces.Genie;

namespace MigrationTool.Services.Implementations.Genie
{
    internal class GenieSettingsService : SettingsServiceBase, IGenieSettingsService
    {
        private const string GenieIPKey = "GenieIP";
        private const string GeniePortKey = "GeniePort";
        private const string GenieUsernameKey = "GenieUsername";

        private const string GenieXmlExportPathKey = "GenieXmlExportPath";
        private const string GenieDocumentsPathKey = "GenieDocumentsPath";
        private const string LaboratoryResultsPathKey = "LaboratoryResultsPath";
        private const string Md3XmlPathKey = "Md3XmlPath";

        public string IP
        {
            get { return ConfigurationManager.AppSettings[GenieIPKey]; }
            set { SaveInConfigFile(GenieIPKey, value); }
        }

        public string Port
        {
            get { return ConfigurationManager.AppSettings[GeniePortKey]; }
            set { SaveInConfigFile(GeniePortKey, value); }
        }

        public string Username
        {
            get { return ConfigurationManager.AppSettings[GenieUsernameKey]; }
            set { SaveInConfigFile(GenieUsernameKey, value); }
        }

        public Func<string> Password { get; set; }

        public string XmlExportPath
        {
            get { return ConfigurationManager.AppSettings[GenieXmlExportPathKey]; }
            set { SaveInConfigFile(GenieXmlExportPathKey, value); }
        }

        public string Md3XmlPath
        {
            get { return ConfigurationManager.AppSettings[Md3XmlPathKey]; }
            set { SaveInConfigFile(Md3XmlPathKey, value); }
        }

        public string DocumentsPath
        {
            get { return ConfigurationManager.AppSettings[GenieDocumentsPathKey]; }
            set { SaveInConfigFile(GenieDocumentsPathKey, value); }
        }

        public string LaboratoryResultsPath
        {
            get { return ConfigurationManager.AppSettings[LaboratoryResultsPathKey]; }
            set { SaveInConfigFile(LaboratoryResultsPathKey, value); }
        }

        public string ConnectionString
        {
            get
            {
                var pairs = new[]
                {
                    new KeyValuePair<string, string>("driver", "{4D v14 ODBC Driver 64 bits}"),
                    new KeyValuePair<string, string>("server", IP),
                    new KeyValuePair<string, string>("port", Port),
                    new KeyValuePair<string, string>("uid", Username),
                    new KeyValuePair<string, string>("pwd", Password()),
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