using System.Linq;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using ServiceLogic.Interfaces;
using ServiceLogic.Models;

namespace ServiceLogic.Services;

public sealed class SalesSnapshotService : ISalesSnapshotService
{
    private readonly ITableCache _tableCache;
    private readonly ILogger<SalesSnapshotService> _logger;

    public SalesSnapshotService(
        ITableCache tableCache,
        ILogger<SalesSnapshotService> logger)
    {
        _tableCache = tableCache;
        _logger = logger;
    }

    public async Task<IReadOnlyList<SalesOrderSummary>> GetRecentOrdersAsync(int top, CancellationToken cancellationToken)
    {
        var limit = top <= 0 ? 10 : Math.Min(top, 100);

        var entries = await _tableCache.GetAsync<SalesOrderEntry>(cancellationToken);
        if (entries.Count == 0)
        {
            _logger.LogInformation("Sales orders cache empty, attempting refresh.");
            await _tableCache.RefreshAsync<SalesOrderEntry>(cancellationToken);
            entries = await _tableCache.GetAsync<SalesOrderEntry>(cancellationToken);
        }

        return entries
            .OrderByDescending(entry => entry.OrderDate)
            .Take(limit)
            .Select(entry => new SalesOrderSummary(entry.CustomerName, entry.OrderDate, entry.TotalAmount))
            .ToArray();
    }
}
