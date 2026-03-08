using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using Microsoft.EntityFrameworkCore;

namespace LiTest.Server.Infrastructure
{
    public class LiTestDbContext : DbContext
    {
        public DbSet<UserEntity> Users;
        public DbSet<LiTestEntity> Tests;
        public LiTestDbContext(DbContextOptions<LiTestDbContext> options)
            : base(options)
        {

        }
    }
}
