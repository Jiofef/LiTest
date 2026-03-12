using LiTest.Server.Core.Contracts.Data;
using Microsoft.EntityFrameworkCore;


namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository : ILiTestRepository
    {
        private readonly IDbContextFactory<LiTestDbContext> _ctxFactory;

        public LiTestRepository(IDbContextFactory<LiTestDbContext> contextFactory)
        {
            _ctxFactory = contextFactory;
        }
    }
}
