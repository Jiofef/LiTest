using LiTest.Shared.Core.Community;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Infrastructure
{
    public class LiTestRepository
    {
        private readonly DbContextFactory<LiTestDbContext> _ctxFactory;

        public async Task <List<UserEntity>> GetUsers(Guid ids)
        {
            var ctx = _ctxFactory.CreateDbContextAsync();
        }
    }
}
