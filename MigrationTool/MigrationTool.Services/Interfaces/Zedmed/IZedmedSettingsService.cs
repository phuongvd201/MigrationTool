using System;

namespace MigrationTool.Services.Interfaces.Zedmed
{
    public interface IZedmedSettingsService
    {
        string Username { get; set; }

        Func<string> Password { get; set; }

        string DatabaseFolderPath { get; set; }

        string DocumentsPath { get; set; }

        string ClinplusConnectionString { get; }

        string SuperplusConnectionString { get; }
    }
}