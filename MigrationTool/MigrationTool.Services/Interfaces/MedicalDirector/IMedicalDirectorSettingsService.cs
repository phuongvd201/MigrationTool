namespace MigrationTool.Services.Interfaces.MedicalDirector
{
    public interface IMedicalDirectorSettingsService
    {
        string DatabaseServerName { get; set; }

        string DocumentsPath { get; set; }

        string HcnConnectionString { get; }

        string ContentConnectionString { get; }
    }
}