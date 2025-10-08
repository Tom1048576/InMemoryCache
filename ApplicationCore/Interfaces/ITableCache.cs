namespace ApplicationCore.Interfaces;

public interface ITableCache
{
    Task<IReadOnlyList<TEntry>> GetAsync<TEntry>(CancellationToken cancellationToken) where TEntry : class;

    Task RefreshAsync<TEntry>(CancellationToken cancellationToken) where TEntry : class;

    Task RefreshAllAsync(CancellationToken cancellationToken);
}
