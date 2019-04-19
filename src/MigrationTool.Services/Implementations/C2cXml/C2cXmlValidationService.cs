using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces.C2cXml;

using Siberia.Migration.Serialization.Xml;

using Xml = Siberia.Migration.Serialization.Xml.C2cXml;

namespace MigrationTool.Services.Implementations.C2cXml
{
    internal class C2cXmlValidationService : IC2cXmlValidationService
    {
        public IC2cXmlSettingsService C2cXmlSettingsService { get; set; }

        public string Validate()
        {
            var validationResult = string.Empty;
            var schemaVersionCheckResult = string.Empty;

            Xml.ReadThrough()
                .WithValidation(
                    x => validationResult = x,
                    x => schemaVersionCheckResult = x)
                .FromXml()
                .FromFile(C2cXmlSettingsService.C2cXmlPath);

            if (!string.IsNullOrWhiteSpace(validationResult)
                && !string.IsNullOrWhiteSpace(schemaVersionCheckResult))
            {
                validationResult += "\r\n\r\n" + schemaVersionCheckResult;
            }
            return validationResult;
        }
    }
}