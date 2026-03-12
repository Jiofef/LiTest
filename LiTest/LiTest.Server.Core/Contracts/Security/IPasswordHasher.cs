using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Core.Contracts.Security
{
    public interface IPasswordHasher
    {
        string Generate(string password);
        bool Verify(string password, string hashedPassword);
    }
}
