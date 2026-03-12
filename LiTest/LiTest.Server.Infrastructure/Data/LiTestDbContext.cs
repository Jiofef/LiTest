using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using Microsoft.EntityFrameworkCore;

namespace LiTest.Server.Infrastructure.Data
{
    public class LiTestDbContext : DbContext
    {
        public DbSet<UserEntity> Users;
        public DbSet<LiTestEntity> Tests;
        public LiTestDbContext(DbContextOptions<LiTestDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Users
            modelBuilder.Entity<UserEntity>()
                .HasIndex(u => u.Login)
                .IsUnique();

            // Litests
            modelBuilder.Entity<LiTestEntity>()
                .HasIndex(lt => new { lt.PassersCount, lt.Id })
                .HasDatabaseName("IX_LiTest_Pagination");

            modelBuilder.HasPostgresExtension("pg_trgm");
            modelBuilder.Entity<LiTestEntity>()
                .HasIndex(lt => lt.Name)
                .HasMethod("gin")
                .HasOperators("gin_trgm_ops");

            modelBuilder.Entity<LiTestEntity>()
                .HasIndex(e => e.Tags)
                .HasMethod("gin");

            modelBuilder.Entity<LiTestEntity>()
                .HasIndex(p => new { p.PublishedAt, p.Id })
                .HasDatabaseName("IX_Posts_PublishedAt_Id")
                .IsDescending(true, true);

            modelBuilder.Entity<LiTestEntity>()
                .Property(p => p.PublishedAt)
                .HasColumnType("timestamp(3) with time zone");

            base.OnModelCreating(modelBuilder);
        }
    }
}
