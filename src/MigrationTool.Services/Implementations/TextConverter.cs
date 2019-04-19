using System;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using MigrationTool.Services.Helpers;
using MigrationTool.Services.Helpers.Text;
using MigrationTool.Services.Interfaces;

using Log4net = log4net;

namespace MigrationTool.Services.Implementations
{
    internal class TextConverter : ITextConverter
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public string TextToHtml(string text)
        {
            return text.IsRtfTextFormat() ? RtfToHtml(text) : PitToHtml(text);
        }

        public string TextToDecodedHtml(string text)
        {
            var htmlText = text.IsRtfTextFormat() ? RtfToHtml(text) : text;

            return WebUtility.HtmlDecode(htmlText);
        }

        public string RtfToHtml(string rtf)
        {
            return StartSTATask(() => SafeRtfToHtml(rtf)).Result;
        }

        private string PitToHtml(string pitText)
        {
            return PitToHtmlConverter.ConvertPitToHtml(pitText);
        }

        private string SafeRtfToHtml(string rtf)
        {
            try
            {
                return RtfToHtmlConverter.ConvertRtfToHtml(rtf);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert RTF to HTML", ex);
                return string.Empty;
            }
        }

        private static Task<T> StartSTATask<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            var thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(func());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                finally
                {
                    Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
                    Dispatcher.Run();
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }
    }
}