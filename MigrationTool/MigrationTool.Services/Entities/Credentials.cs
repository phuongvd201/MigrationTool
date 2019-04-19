using System;

namespace MigrationTool.Services.Entities
{
    public class Credentials
    {
        public string Username { get; set; }

        public Func<string> Password { get; set; }
    }
}