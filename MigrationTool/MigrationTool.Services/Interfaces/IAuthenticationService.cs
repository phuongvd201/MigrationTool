using System;

using MigrationTool.Services.Entities;

namespace MigrationTool.Services.Interfaces
{
    public interface IAuthenticationService
    {
        string Login(Credentials credentials);

        Func<string> GetAuth { get; }

        Func<string> GetCookieName { get; }
    }
}