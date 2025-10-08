using System.Collections.Generic;

namespace Infrastructure.Options;

public sealed class DatabaseFleetOptions
{
    public const string SectionName = "DatabaseFleet";

    public IList<DatabaseSourceOptions> Sources { get; init; } = new List<DatabaseSourceOptions>();
}

public sealed class DatabaseSourceOptions
{
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Friendly description for the origin of the data (table, view, or stored procedure).
    /// </summary>
    public string Source { get; init; } = string.Empty;

    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Query that projects the rows you intend to cache. Keep it deterministic because the rows lack primary keys.
    /// </summary>
    public string Query { get; init; } = string.Empty;
}
