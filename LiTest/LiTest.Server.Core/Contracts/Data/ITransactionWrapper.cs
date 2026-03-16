namespace LiTest.Server.Core.Contracts.Data
{
    public interface ITransactionWrapper : IDisposable, IAsyncDisposable
    {
        Task CommitAsync();
        Task RollbackAsync();
    }
}