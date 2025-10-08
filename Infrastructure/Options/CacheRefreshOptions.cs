namespace Infrastructure.Options;

public sealed class CacheRefreshOptions
{
    public const string SectionName = "CacheRefresh";

    /// <summary>
    /// How frequently the background refresher forces a reload of the fleet data.
    /// Defaults to 4 hours.
    /// </summary>
    public TimeSpan Interval { get; init; } = TimeSpan.FromHours(4);

    /// <summary>
    /// Absolute expiration for the cache. Defaults to 5 hours as per requirements.
    /// </summary>
    public TimeSpan AbsoluteExpiration { get; init; } = TimeSpan.FromHours(5);
}
