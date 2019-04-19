using System.Configuration;

using MigrationTool.Services.Interfaces;

namespace MigrationTool.Services.Implementations
{
    internal class AppSettingsService : IAppSettingsService
    {
        private const string PreferredBatchSizeInBytesKey = "PreferredBatchSizeInBytes";

        private const string ParallelBatchesNumberKey = "ParallelBatchesNumber";

        private const string RetryCountKey = "RetryCount";

        private const string BaseUrlKey = "SiberiaBaseUrl";

        private readonly int localRetryCount;

        private readonly int localPreferredBatchSizeInBytes;

        private readonly int localParallelBatchesNumber;

        public string Version
        {
            get { return typeof(AppSettingsService).Assembly.GetName().Version.ToString(3); }
        }

        public string SiberiaBaseUrl
        {
            get
            {
                var result = ConfigurationManager.AppSettings[BaseUrlKey];
                return string.IsNullOrWhiteSpace(result) ? "https://www.clinictocloud.com.au" : result;
            }
        }

        public int RetryCount
        {
            get { return localRetryCount; }
        }

        public int PreferredBatchSizeInBytes
        {
            get { return localPreferredBatchSizeInBytes; }
        }

        public int ParallelBatchesNumber
        {
            get { return localParallelBatchesNumber; }
        }

        public string SiberiaLoginUrl
        {
            get { return "/api/v1/user/LoginMigrationTool"; }
        }

        public string SiberiaRegisterMigrationUrl
        {
            get { return "/api/v1/migration/RegisterMigration"; }
        }

        public string SiberiaCommitMigrationUrl
        {
            get { return "/api/v1/migration/CommitMigration"; }
        }

        public string SiberiaGetMigrationDataSourcesUrl
        {
            get { return "/api/v1/migration/GetMigrationDataSources"; }
        }

        public string SiberiaCreateMigrationDataSourceUrl
        {
            get { return "/api/v1/migration/CreateMigrationDataSource"; }
        }

        public string SiberiaUploadFileUrl
        {
            get { return "/api/v1/migration/UploadMigrationFile"; }
        }

        public AppSettingsService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings[RetryCountKey], out localRetryCount))
            {
                localRetryCount = 3;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings[ParallelBatchesNumberKey], out localParallelBatchesNumber))
            {
                localParallelBatchesNumber = 3;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings[PreferredBatchSizeInBytesKey], out localPreferredBatchSizeInBytes))
            {
                localPreferredBatchSizeInBytes = 100 * 1024 * 1024;
            }
        }
    }
}