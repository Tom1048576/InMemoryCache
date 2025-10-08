using System.Collections.Concurrent;
using System.Linq;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Caching;

public sealed class TableCache : ITableCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly IReadOnlyDictionary<Type, ITableDataLoader> _loaders;
    private readonly CacheRefreshOptions _options;
    private readonly ILogger<TableCache> _logger;
    private readonly ConcurrentDictionary<Type, SemaphoreSlim> _locks = new();

    public TableCache(
        IMemoryCache memoryCache,
        IEnumerable<ITableDataLoader> loaders,
        IOptions<CacheRefreshOptions> options,
        ILogger<TableCache> logger)
    {
        _memoryCache = memoryCache;
        _options = options.Value;
        _logger = logger;
        _loaders = loaders.ToDictionary(loader => loader.EntryType);
    }

    public async Task<IReadOnlyList<TEntry>> GetAsync<TEntry>(CancellationToken cancellationToken) where TEntry : class
    {
        var cacheKey = TableMetadataExtensions.GetTableMetadata<TEntry>().TableKey;

        if (_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<TEntry>? cached) && cached is not null)
        {
            return cached;
        }

        await RefreshAsync<TEntry>(cancellationToken);

        if (_memoryCache.TryGetValue(cacheKey, out cached) && cached is not null)
        {
            return cached;
        }

        return Array.Empty<TEntry>();
    }

    public async Task RefreshAsync<TEntry>(CancellationToken cancellationToken) where TEntry : class
    {
        var entryType = typeof(TEntry);
        if (!_loaders.TryGetValue(entryType, out var loader))
        {
            _logger.LogWarning("No loader registered for entry type {EntryType}.", entryType.FullName);
            return;
        }

        var semaphore = _locks.GetOrAdd(entryType, _ => new SemaphoreSlim(1, 1));

        if (!await semaphore.WaitAsync(0, cancellationToken))
        {
            _logger.LogDebug("Refresh already in progress for {EntryType}.", entryType.Name);
            return;
        }

        try
        {
            if (loader is not ITableDataLoader<TEntry> typedLoader)
            {
                _logger.LogWarning("Loader registration for {EntryType} does not support typed refresh.", entryType.FullName);
                return;
            }
            var entries = await typedLoader.LoadAsync(cancellationToken);

            var cacheKey = TableMetadataExtensions.GetTableMetadata<TEntry>().TableKey;
            _memoryCache.Set(cacheKey, entries, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _options.AbsoluteExpiration
            });

            _logger.LogInformation("Cached {Count} entries for {TableKey}.", entries.Count, cacheKey);
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Failed to refresh cache for entry type {EntryType}.", entryType.FullName);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task RefreshAllAsync(CancellationToken cancellationToken)
    {
        foreach (var loader in _loaders.Values)
        {
            var method = typeof(TableCache).GetMethod(nameof(RefreshAsync))?
                .MakeGenericMethod(loader.EntryType);

            if (method is null)
            {
                continue;
            }

            var task = (Task?)method.Invoke(this, new object[] { cancellationToken });
            if (task is not null)
            {
                await task.ConfigureAwait(false);
            }
        }
    }
}
