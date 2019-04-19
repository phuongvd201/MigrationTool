namespace MigrationTool.Services.Interfaces
{
    internal interface ITextConverter
    {
        string RtfToHtml(string text);

        string TextToHtml(string text);

        string TextToDecodedHtml(string text);
    }
}