using System.Reflection;

using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces;

using RestSharp;

using Siberia.Migration.Entities.Common;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations
{
    internal class MigrationSourceService : IMigrationSourceService
    {
        private static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IAppSettingsService AppSettings { get; set; }

        public IAuthenticationService AuthenticationService { get; set; }

        public string[] GetPreviousSourceNames(MigrationSourceSystem sourceSystem)
        {
            Log.Info("Requesting previously used source names.");

            var client = new RestClient(AppSettings.SiberiaBaseUrl);
            var request = new RestRequest(AppSettings.SiberiaGetMigrationDataSourcesUrl, Method.POST);

            request.AddCookie(AuthenticationService.GetCookieName(), AuthenticationService.GetAuth());

            request.AddParameter("MigrationSourceSystem", (int)sourceSystem);

            var response = client.Execute(request);
            return response.InterpretResponse<string[]>(Log, "request source names");
        }

        public string CreateSource(MigrationSourceSystem sourceSystem, string sourceName)
        {
            Log.Info("Requesting creation of a new data source.");

            var client = new RestClient(AppSettings.SiberiaBaseUrl);
            var request = new RestRequest(AppSettings.SiberiaCreateMigrationDataSourceUrl, Method.POST);

            request.AddCookie(AuthenticationService.GetCookieName(), AuthenticationService.GetAuth());

            request.AddParameter("MigrationSourceSystem", (int)sourceSystem);
            request.AddParameter("MigrationSourceName", sourceName);

            var response = client.Execute(request);
            return response.InterpretResponse<bool>(Log, "create source name") ? null : "Failed to create a new data source.";
        }
    }
}