using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

using DocumentFormat.OpenXml.Packaging;

using OpenXmlPowerTools;

using Log4net = log4net;

namespace MigrationTool.Services.Helpers.Text
{
    internal class WordToHtmlConverter
    {
        protected static readonly Log4net.ILog Log = Log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string ConvertWordToHtml(string wordFilePath)
        {
            try
            {
                byte[] wordData = File.Exists(wordFilePath) ? File.ReadAllBytes(wordFilePath) : null;
                return ConvertWordToHtml(wordData);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert WORD to HTML : " + wordFilePath, ex);
                return null;
            }
        }

        public static string ConvertWordToHtml(byte[] wordData)
        {
            if (wordData == null)
            {
                return string.Empty;
            }

            using (var wordMemoryStream = new MemoryStream())
            {
                wordMemoryStream.Write(wordData, 0, wordData.Length);
                using (var wordDoc = WordprocessingDocument.Open(wordMemoryStream, true))
                {
                    var settings = new HtmlConverterSettings();
                    return HtmlConverter.ConvertToHtml(wordDoc, settings).ToString(SaveOptions.DisableFormatting);
                }
            }
        }
    }
}