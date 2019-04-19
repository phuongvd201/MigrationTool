using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

using MigrationTool.Services.Entities;
using MigrationTool.Services.Helpers;
using MigrationTool.Services.Interfaces;

using RestSharp;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations
{
    internal class AuthenticationService : IAuthenticationService
    {
        private static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public IAppSettingsService AppSettings { get; set; }

        public Func<string> GetAuth { get; private set; }

        public Func<string> GetCookieName { get; private set; }

        static AuthenticationService()
        {
            bool handleSsl;
            if (bool.TryParse(ConfigurationManager.AppSettings["HandleSsl"], out handleSsl) && handleSsl)
            {
                ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;
            }
        }

        private static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public AuthenticationService()
        {
            GetCookieName = () => ".ASPXAUTH";
        }

        public string Login(Credentials credentials)
        {
            var username = credentials.Username;
            var password = credentials.Password();
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return "Username or password is empty";
            }

            var client = new RestClient(AppSettings.SiberiaBaseUrl);
            var request = new RestRequest(AppSettings.SiberiaLoginUrl, Method.POST);

            request.AddParameter("UserName", username);
            request.AddParameter("Password", password);
            request.AddParameter("Version", AppSettings.Version);

            Log.Info("Logging in");

            var response = client.Execute(request);
            var cookie = response.Cookies.FirstOrDefault(x => x.Name == GetCookieName());

            if (response.ErrorException == null && response.StatusCode == HttpStatusCode.OK && cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
            {
                GetAuth = () => cookie.Value;

                Log.Info("Success: logged in");

                return null;
            }

            ApiResponse<string> apiResponse;
            var errorMessage = response.Content.TryParseJson(Log, out apiResponse) ? apiResponse.ErrorMessage : "Login error";

            if (response.ErrorException != null)
            {
                Log.Error(response.ErrorException);
            }
            else
            {
                Log.WarnFormat(errorMessage);
            }

            GetAuth = () => null;
            return errorMessage;
        }
    }
}