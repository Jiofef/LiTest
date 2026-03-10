using LiTest.Server.Infrastructure.Testing;
using LiTest.Shared.Core.Community;
using LiTest.Shared.Core.Testing;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository
    {
        private readonly IDbContextFactory<LiTestDbContext> _ctxFactory;

        public LiTestRepository(IDbContextFactory<LiTestDbContext> contextFactory)
        {
            _ctxFactory = contextFactory;
        }
    }
}
