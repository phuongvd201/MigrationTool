using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

using MigrationTool.Services.Entities;

namespace MigrationTool.Services.Helpers
{
    public static class CommonExtensions
    {
        public static string Md5(this byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            using (var md5 = MD5.Create())
            {
                return Convert.ToBase64String(md5.ComputeHash(data));
            }
        }

        public static string Translate(this string entry, IDictionary<string, string> dictionary)
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                return null;
            }

            if (!dictionary.ContainsKey(entry))
            {
                return entry;
            }

            var translatedEntry = dictionary[entry];

            return translatedEntry ?? entry;
        }

        public static ParsedName ParseName(this string fullName, string[] knownSalutations)
        {
            var salutation = knownSalutations.FirstOrDefault(x => fullName.StartsWith(x, StringComparison.InvariantCultureIgnoreCase));

            var fullnameArray = (salutation != null
                ? fullName.Substring(salutation.Length)
                : fullName).Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            return new ParsedName
            {
                Salutation = salutation,
                FirstName = fullnameArray.FirstOrDefault(),
                LastName = string.Join(" ", fullnameArray.Skip(1).ToArray()),
            };
        }
    }
}