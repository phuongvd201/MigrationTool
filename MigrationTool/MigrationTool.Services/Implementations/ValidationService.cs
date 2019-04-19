using System.IO;
using System.Linq;

using MigrationTool.Services.Interfaces;
using MigrationTool.Services.Interfaces.C2cXml;
using MigrationTool.Services.Interfaces.Genie;
using MigrationTool.Services.Interfaces.MedicalDirector;
using MigrationTool.Services.Interfaces.Shexie;
using MigrationTool.Services.Interfaces.Zedmed;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Implementations
{
    internal class ValidationService : IValidationService
    {
        public IGenieSettingsService GenieSettingsService { get; set; }

        public IShexieSettingsService ShexieSettingsService { get; set; }

        public IC2cXmlSettingsService C2cXmlSettingsService { get; set; }

        public IZedmedSettingsService ZedmedSettingsService { get; set; }

        public IMedicalDirectorSettingsService MedicalDirectorSettingsService { get; set; }

        public string[] ValidateSettings(MigrationSourceSystem sourceSystem, MigrationEntity[] selectedEntities)
        {
            var results = Enumerable.Empty<string>();

            if (!selectedEntities.Any())
            {
                return new[] { "Please, select entities you would like to migrate" };
            }

            switch (sourceSystem)
            {
                case MigrationSourceSystem.Genie:
                    if (selectedEntities.Contains(MigrationEntity.Documents) && !DirectoryPathValid(GenieSettingsService.DocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate documents: Documents directory not specified" });
                    }
                    var lettersMigrationRequested = selectedEntities.Contains(MigrationEntity.Letters) || selectedEntities.Contains(MigrationEntity.IncomingLetters);
                    if (lettersMigrationRequested && !DirectoryPathValid(GenieSettingsService.XmlExportPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate letters: Xml directory not specified" });
                    }
                    break;

                case MigrationSourceSystem.Shexie:
                    if (selectedEntities.Contains(MigrationEntity.Documents) && !DirectoryPathValid(ShexieSettingsService.DocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate documents: Documents directory not specified" });
                    }
                    if (selectedEntities.Contains(MigrationEntity.Letters) && !DirectoryPathValid(ShexieSettingsService.DocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate letters: Documents directory not specified" });
                    }
                    break;

                case MigrationSourceSystem.C2cXml:
                    if (selectedEntities.Contains(MigrationEntity.Documents) && !DirectoryPathValid(C2cXmlSettingsService.C2cDocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate documents: Documents directory not specified" });
                    }
                    break;

                case MigrationSourceSystem.Zedmed:
                    if (selectedEntities.Contains(MigrationEntity.Documents) && !DirectoryPathValid(ZedmedSettingsService.DocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate documents: Documents directory not specified" });
                    }
                    break;

                case MigrationSourceSystem.MedicalDirector:
                    if (selectedEntities.Contains(MigrationEntity.Documents) && !DirectoryPathValid(MedicalDirectorSettingsService.DocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate documents: Documents directory not specified" });
                    }
                    if (selectedEntities.Contains(MigrationEntity.Letters) && !DirectoryPathValid(MedicalDirectorSettingsService.DocumentsPath))
                    {
                        results = results.Concat(new[] { "Unable to migrate letters: Documents directory not specified" });
                    }
                    break;
                default:
                    break;
            }

            return results.Concat(ValidateSettings(sourceSystem)).ToArray();
        }

        public string[] ValidateSettings(MigrationSourceSystem sourceSystem)
        {
            var results = Enumerable.Empty<string>();

            switch (sourceSystem)
            {
                case MigrationSourceSystem.C2cXml:
                    if (!FilePathValid(C2cXmlSettingsService.C2cXmlPath))
                    {
                        results = results.Concat(new[] { "Please, specify the source C2C XML file." });
                    }
                    break;

                case MigrationSourceSystem.Zedmed:
                    if (!DirectoryPathValid(ZedmedSettingsService.DatabaseFolderPath))
                    {
                        results = results.Concat(new[] { "Please, specify the source ZedMed folder." });
                    }
                    break;

                default:
                    break;
            }

            return results.ToArray();
        }

        public string[] ValidateDataSourceName(string dataSourceName)
        {
            if (string.IsNullOrWhiteSpace(dataSourceName))
            {
                return new[] { "Please, provide a non-empty source name" };
            }

            return new string[] { };
        }

        private bool DirectoryPathValid(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        }

        private bool FilePathValid(string path)
        {
            return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
        }
    }
}