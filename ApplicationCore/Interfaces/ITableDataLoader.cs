using System.Collections.Generic;

namespace ApplicationCore.Interfaces;

public interface ITableDataLoader
{
    Type EntryType { get; }

    string TableKey { get; }

    Task<IReadOnlyList<object>> LoadUntypedAsync(CancellationToken cancellationToken);
}

public interface ITableDataLoader<TEntry> : ITableDataLoader where TEntry : class
{
    Task<IReadOnlyList<TEntry>> LoadAsync(CancellationToken cancellationToken);
}
