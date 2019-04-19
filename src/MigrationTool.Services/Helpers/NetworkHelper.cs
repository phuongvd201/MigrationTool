using System;
using System.IO;
using System.Net;

using log4net;

using MigrationTool.Services.Entities;

using RestSharp;

using Siberia.Migration.Entities.Common;

namespace MigrationTool.Services.Helpers
{
    internal static class NetworkHelper
    {
        public static Exception ToMigrationFileServer(
            this Action<Stream> useStream,
            string clientUrl,
            string methodUrl,
            Func<string> getCookie,
            Func<string> getAuth,
            Guid migrationGuid,
            MigrationEntity migrationEntity,
            string filename,
            int retriesLeft)
        {
            if (retriesLeft < 1)
            {
                throw new ArgumentOutOfRangeException("retriesLeft", "Retry count cannot be negative or zero.");
            }

            try
            {
                var client = new RestClient(clientUrl);
                var request = new RestRequest(methodUrl, Method.POST);
                request.Timeout = 60 * 60 * 1000;
                request.AddCookie(getCookie(), getAuth());
                request.AddParameter("MigrationGuid", migrationGuid);
                request.AddParameter("MigrationEntity", (int)migrationEntity);
                request.AddFile(
                    "MigrationData",
                    useStream,
                    Path.GetFileName(filename),
                    "application/json"
                    );

                var response = client.Execute(request);
                if (response.ErrorException != null || response.StatusCode != HttpStatusCode.OK)
                {
                    if (retriesLeft == 1)
                    {
                        return new Exception("Error when sending data.", response.ErrorException);
                    }

                    return useStream.ToMigrationFileServer(
                        clientUrl,
                        methodUrl,
                        getCookie,
                        getAuth,
                        migrationGuid,
                        migrationEntity,
                        filename,
                        retriesLeft - 1);
                }

                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static T InterpretResponse<T>(this IRestResponse response, ILog log, string processName)
        {
            ApiResponse<T> apiResponse;

            if (response.ErrorException != null)
            {
                log.Error(response.StatusCode.GetResponseLogMessage(processName, response.ErrorException.Message), response.ErrorException);
            }
            else if (!response.Content.TryParseJson(log, out apiResponse))
            {
                log.Error(response.StatusCode.GetResponseLogMessage(processName, response.Content));
            }
            else if (!string.IsNullOrWhiteSpace(apiResponse.ErrorMessage))
            {
                log.Error(response.StatusCode.GetResponseLogMessage(processName, apiResponse.ErrorMessage));
            }
            else
            {
                log.Info(response.StatusCode.GetResponseLogMessage(processName, null));
                return apiResponse.Result;
            }

            return default(T);
        }

        private static string GetResponseLogMessage(this HttpStatusCode statusCode, string processName, string message)
        {
            var result = statusCode == HttpStatusCode.OK
                ? "Success: " + processName + "."
                : "Failed: " + processName + ". Status: " + (int)statusCode + ".";

            result += string.IsNullOrWhiteSpace(message)
                ? string.Empty
                : " Message: " + message;

            return result;
        }
    }
}