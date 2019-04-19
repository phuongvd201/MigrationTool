using System.IO;

using Siberia.Migration.Entities;

namespace MigrationTool.Services.Helpers.C2cXml
{
    internal static class C2cXmlMigrationHelper
    {
        public static MigrationDocument LoadFileData(this MigrationDocument source, string folderPath)
        {
            if (source == null || !source.FileName.IsValidFileName())
            {
                return source;
            }

            var filePath = Path.Combine(folderPath, source.FileName);
            if (!File.Exists(filePath))
            {
                return source;
            }

            source.FileData = File.ReadAllBytes(filePath);
            source.Md5 = source.FileData.Md5();

            return source;
        }
    }
}