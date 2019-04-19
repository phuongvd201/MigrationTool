using System;

namespace MigrationTool.Services.Interfaces.Genie
{
    public interface IGenieSettingsService
    {
        string IP { get; set; }

        string Port { get; set; }

        string Username { get; set; }

        Func<string> Password { get; set; }

        string XmlExportPath { get; set; }

        string Md3XmlPath { get; set; }

        string DocumentsPath { get; set; }

        string LaboratoryResultsPath { get; set; }

        string ConnectionString { get; }
    }
}