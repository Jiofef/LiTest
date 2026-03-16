using LiTest.Server.Core.Contracts.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LiTest.Server.Infrastructure.Data
{
    public partial class LiTestRepository : ILiTestRepository
    {
        private readonly LiTestDbContext _context;

        public LiTestRepository(LiTestDbContext context)
        {
            _context = context;
        }

        public async Task<ITransactionWrapper> BeginTransactionAsync()
        {
            var tx = await _context.Database.BeginTransactionAsync();
            return new EfTransactionWrapper(tx);
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}