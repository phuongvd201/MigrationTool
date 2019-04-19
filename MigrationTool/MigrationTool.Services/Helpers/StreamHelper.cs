using System;
using System.IO;

namespace MigrationTool.Services.Helpers
{
    public static class StreamHelper
    {
        public static void AsString(this Action<Stream> useStream, Action<string> action)
        {
            string result;
            using (var stream = new MemoryStream())
            {
                useStream(stream);
                stream.Seek(0, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(stream))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            action(result);
        }

        public static void AsBytes(this Action<Stream> useStream, Action<byte[]> action)
        {
            byte[] result;
            using (var stream = new MemoryStream())
            {
                useStream(stream);
                stream.Seek(0, SeekOrigin.Begin);
                result = stream.ToArray();
            }
            action(result);
        }
    }
}