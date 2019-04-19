using System.Configuration;

using MigrationTool.Services.Interfaces.C2cXml;

namespace MigrationTool.Services.Implementations.C2cXml
{
    internal class C2cXmlSettingsService : SettingsServiceBase, IC2cXmlSettingsService
    {
        private const string C2cXmlPathKey = "C2cXmlPath";
        private const string C2cDocumentsPathKey = "C2cDocumentsPath";

        public string C2cXmlPath
        {
            get { return ConfigurationManager.AppSettings[C2cXmlPathKey]; }
            set { SaveInConfigFile(C2cXmlPathKey, value); }
        }

        public string C2cDocumentsPath
        {
            get { return ConfigurationManager.AppSettings[C2cDocumentsPathKey]; }
            set { SaveInConfigFile(C2cDocumentsPathKey, value); }
        }
    }
}