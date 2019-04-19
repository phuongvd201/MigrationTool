using System;
using System.IO;
using System.Linq;

using ICSharpCode.SharpZipLib.Zip;

namespace MigrationTool.Services.Helpers
{
    internal static class StorageHelper
    {
        private const int ZipBufferSize = 4096;

        public static void FromFile(this Action<Stream> useStream, string fileName)
        {
            if (useStream == null)
            {
                throw new ArgumentNullException("useStream");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentOutOfRangeException("fileName", "The file name is empty.");
            }
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                useStream(stream);
            }
        }

        public static void ToFile(this Action<Stream> useStream, string fileName)
        {
            if (useStream == null)
            {
                throw new ArgumentNullException("useStream");
            }
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentOutOfRangeException("fileName", "The file name is empty.");
            }
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                useStream(stream);
            }
        }

        public static Action<Stream> UseStream(Action<Action<Stream>> useStream)
        {
            return stream =>
            {
                useStream(x =>
                {
                    stream.Position = 0;
                    stream.CopyTo(x);
                });
            };
        }

        public static Action<Stream> FromZip(this Action<Stream> useStream, string zipEntryName)
        {
            return stream =>
            {
                using (var outputStream = new MemoryStream())
                {
                    using (var zipFile = new ZipFile(stream))
                    {
                        var zipEntry = zipFile.Cast<ZipEntry>().FirstOrDefault(x => x.Name.EqualsIgnoreCase(zipEntryName));
                        if (zipEntry != null && zipEntry.Size > 0)
                        {
                            using (var zippedStream = zipFile.GetInputStream(zipEntry))
                            {
                                var dataBuffer = new byte[ZipBufferSize];
                                int readBytes;
                                while ((readBytes = zippedStream.Read(dataBuffer, 0, ZipBufferSize)) > 0)
                                {
                                    outputStream.Write(dataBuffer, 0, readBytes);
                                }
                            }
                        }
                    }
                    outputStream.Position = 0;
                    useStream(outputStream);
                }
            };
        }
    }
}