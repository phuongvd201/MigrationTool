namespace MigrationTool.Services.Interfaces.Shexie
{
    public interface IShexieSettingsService
    {
        string DatabaseFilePath { get; set; }

        string ConnectionString { get; }

        string DocumentsPath { get; set; }
    }
}