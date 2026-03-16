using LiTest.Server.Core.Contracts.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Infrastructure.Data
{
    public class EfTransactionWrapper : ITransactionWrapper
    {
        private readonly IDbContextTransaction _transaction;
        public EfTransactionWrapper(IDbContextTransaction transaction) => _transaction = transaction;
        public Task CommitAsync() => _transaction.CommitAsync();
        public Task RollbackAsync() => _transaction.RollbackAsync();
        public void Dispose() => _transaction.Dispose();
        public ValueTask DisposeAsync() => _transaction.DisposeAsync();
    }
}
