using System.Linq;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;
using ServiceLogic.Interfaces;
using ServiceLogic.Models;

namespace ServiceLogic.Services;

public sealed class SupportAnalyticsService : ISupportAnalyticsService
{
    private readonly ITableCache _tableCache;
    private readonly ILogger<SupportAnalyticsService> _logger;

    public SupportAnalyticsService(
        ITableCache tableCache,
        ILogger<SupportAnalyticsService> logger)
    {
        _tableCache = tableCache;
        _logger = logger;
    }

    public async Task<IReadOnlyDictionary<string, SupportTicketCount>> GetOpenTicketsByPriorityAsync(CancellationToken cancellationToken)
    {
        var entries = await _tableCache.GetAsync<SupportTicketEntry>(cancellationToken);
        if (entries.Count == 0)
        {
            _logger.LogInformation("Support tickets cache empty, attempting refresh.");
            await _tableCache.RefreshAsync<SupportTicketEntry>(cancellationToken);
            entries = await _tableCache.GetAsync<SupportTicketEntry>(cancellationToken);
        }

        return entries
            .Where(entry => string.Equals(entry.Status, "Open", StringComparison.OrdinalIgnoreCase))
            .GroupBy(entry => entry.Priority ?? "Unknown", StringComparer.OrdinalIgnoreCase)
            .Select(group => new SupportTicketCount(group.Key, group.Count()))
            .ToDictionary(result => result.Priority, result => result, StringComparer.OrdinalIgnoreCase);
    }
}
