using LiTest.Server.Core.Contracts.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Generate(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hashedPassword) =>
            BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}
