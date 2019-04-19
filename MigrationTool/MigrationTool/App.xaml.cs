using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace MigrationTool
{
    public partial class App : Application
    {
        private Mutex mutex;

        public App()
        {
            Exit += OnExit;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // get application GUID as defined in AssemblyInfo.cs
            var applicationId = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value;

            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);

            var securitySettings = new MutexSecurity();
            securitySettings.AddAccessRule(allowEveryoneRule);

            bool createdNew;

            var mutexId = string.Format("Global\\{{{0}}}", applicationId);
            mutex = new Mutex(false, mutexId, out createdNew, securitySettings);

            bool handled;
            try
            {
                handled = mutex.WaitOne(2000, false);
            }
            catch (AbandonedMutexException)
            {
                handled = true;
            }

            if (handled)
            {
                return;
            }

            MessageBox.Show("The second application instance is running. Multiple application instances are not allowed.");

            mutex = null;
            Shutdown();
        }

        private void OnExit(object sender, ExitEventArgs args)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
            }
        }
    }
}