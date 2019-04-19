using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace MigrationTool.Services.Helpers.Text
{
    /// <summary>
    /// Transforming Pathology PIT reports into HTML.
    /// </summary>
    /// <remarks>
    /// Reference PIT format at:
    /// http://www.healthintersections.com.au/?p=930.
    /// </remarks>
    internal class PitToHtmlConverter
    {
        private static readonly string PitTagPattern = @"\R\{0}\R\";

        private static readonly Dictionary<string, string> PitTagToHtmlTag = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "SBLD", "<b>" },
            { "EBLD", "</b>" },
            { "SUND", "<u>" },
            { "EUND", "</u>" },
            { "SBLK", "<b><u><i>" },
            { "EBLK", "</i></u></b>" },
            { "FG00", "<span style=\"color: #000000;\">" },
            { "FG01", "<span style=\"color: #0000FF;\">" },
            { "FG02", "<span style=\"color: #008000;\">" },
            { "FG03", "<span style=\"color: #00FFFF;\">" },
            { "FG04", "<span style=\"color: #FF0000;\">" },
            { "FG05", "<span style=\"color: #FF00FF;\">" },
            { "FG06", "<span style=\"color: #A52A2A;\">" },
            { "FG07", "<span style=\"color: #D3D3D3;\">" },
            { "FG08", "<span style=\"color: #A9A9A9;\">" },
            { "FG09", "<span style=\"color: #ADD8E6;\">" },
            { "FG10", "<span style=\"color: #90EE90;\">" },
            { "FG11", "<span style=\"color: #E0FFFF;\">" },
            { "FG12", "<span style=\"color: #FA8072;\">" },
            { "FG13", "<span style=\"color: #EE82EE;\">" },
            { "FG14", "<span style=\"color: #FFFF00;\">" },
            { "FG15", "<span style=\"color: #FFFFFF;\">" },
            { "FG99", "</span>" },
            { "BG00", "<span style=\"background-color: #000000;\">" },
            { "BG01", "<span style=\"background-color: #0000FF;\">" },
            { "BG02", "<span style=\"background-color: #008000;\">" },
            { "BG03", "<span style=\"background-color: #00FFFF;\">" },
            { "BG04", "<span style=\"background-color: #FF0000;\">" },
            { "BG05", "<span style=\"background-color: #FF00FF;\">" },
            { "BG06", "<span style=\"background-color: #A52A2A;\">" },
            { "BG07", "<span style=\"background-color: #D3D3D3;\">" },
            { "BG08", "<span style=\"background-color: #A9A9A9;\">" },
            { "BG09", "<span style=\"background-color: #ADD8E6;\">" },
            { "BG10", "<span style=\"background-color: #90EE90;\">" },
            { "BG11", "<span style=\"background-color: #E0FFFF;\">" },
            { "BG12", "<span style=\"background-color: #FA8072;\">" },
            { "BG13", "<span style=\"background-color: #EE82EE;\">" },
            { "BG14", "<span style=\"background-color: #FFFF00;\">" },
            { "BG15", "<span style=\"background-color: #FFFFFF;\">" },
            { "BG99", "</span>" },
            { "DFLT", string.Empty },
            { "PIpp", string.Empty },
            { "FOff", string.Empty },
        };

        public static string ConvertPitToHtml(string pitText)
        {
            var builder = new StringBuilder(ReplaceBreaklineByHtmlBreakTag(pitText));

            return PitTagToHtmlTag.Aggregate(
                builder,
                (current, pair) =>
                {
                    var pitTag = string.Format(PitTagPattern, pair.Key);
                    return current.Replace(pitTag, pair.Value);
                })
                .ToString();
        }

        private static string ReplaceBreaklineByHtmlBreakTag(string text)
        {
            var sb = new StringBuilder(WebUtility.HtmlEncode(text));

            return sb.Replace("\r\n", "\r")
                .Replace("\n", "\r")
                .Replace("\r", "<br />\r\n")
                .Replace("  ", " &nbsp;")
                .ToString();
        }
    }
}