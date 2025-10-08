using ApplicationCore.Interfaces;
using Infrastructure.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.HostedServices;

public sealed class TableCacheRefreshService : BackgroundService
{
    private readonly ITableCache _tableCache;
    private readonly CacheRefreshOptions _options;
    private readonly ILogger<TableCacheRefreshService> _logger;

    public TableCacheRefreshService(
        ITableCache tableCache,
        IOptions<CacheRefreshOptions> options,
        ILogger<TableCacheRefreshService> logger)
    {
        _tableCache = tableCache;
        _logger = logger;
        _options = options.Value;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Priming typed table cache on service start.");
        await _tableCache.RefreshAllAsync(cancellationToken);
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = _options.Interval <= TimeSpan.Zero
            ? TimeSpan.FromMinutes(60)
            : _options.Interval;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, stoppingToken);
                await _tableCache.RefreshAllAsync(stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // shutting down
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Background refresh failed.");
            }
        }
    }
}
