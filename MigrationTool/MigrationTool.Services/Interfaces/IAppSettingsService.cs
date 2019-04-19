namespace MigrationTool.Services.Interfaces
{
    public interface IAppSettingsService
    {
        string Version { get; }

        int RetryCount { get; }

        int PreferredBatchSizeInBytes { get; }

        int ParallelBatchesNumber { get; }

        string SiberiaBaseUrl { get; }

        string SiberiaLoginUrl { get; }

        string SiberiaUploadFileUrl { get; }

        string SiberiaRegisterMigrationUrl { get; }

        string SiberiaCommitMigrationUrl { get; }

        string SiberiaGetMigrationDataSourcesUrl { get; }

        string SiberiaCreateMigrationDataSourceUrl { get; }
    }
}