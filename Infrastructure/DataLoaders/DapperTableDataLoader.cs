using System.Linq;
using ApplicationCore.Extensions;
using ApplicationCore.Interfaces;
using Dapper;
using Infrastructure.Factories;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataLoaders;

public abstract class DapperTableDataLoader<TEntry> : ITableDataLoader<TEntry> where TEntry : class
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly ILogger _logger;
    private readonly DatabaseSourceOptions _source;

    protected DapperTableDataLoader(
        string sourceName,
        IDatabaseConnectionFactory connectionFactory,
        IOptions<DatabaseFleetOptions> options,
        ILogger logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
        _source = options.Value.Sources
            .FirstOrDefault(source => string.Equals(source.Name, sourceName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Database source '{sourceName}' not found in configuration.");
    }

    public Type EntryType => typeof(TEntry);

    public string TableKey => TableMetadataExtensions.GetTableMetadata<TEntry>().TableKey;

    public async Task<IReadOnlyList<TEntry>> LoadAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_source.ConnectionString) || string.IsNullOrWhiteSpace(_source.Query))
        {
            _logger.LogWarning("Source {SourceName} skipped because it lacks a connection string or query.", _source.Name);
            return Array.Empty<TEntry>();
        }

        try
        {
            await using var connection = await _connectionFactory.CreateOpenConnectionAsync(_source.ConnectionString, cancellationToken);
            var command = new CommandDefinition(_source.Query, cancellationToken: cancellationToken);
            var rows = await connection.QueryAsync<TEntry>(command);
            return rows.ToList();
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Failed to load data for table {TableKey}.", TableKey);
            return Array.Empty<TEntry>();
        }
    }

    async Task<IReadOnlyList<object>> ITableDataLoader.LoadUntypedAsync(CancellationToken cancellationToken)
    {
        var typed = await LoadAsync(cancellationToken).ConfigureAwait(false);
        return typed.Cast<object>().ToList();
    }
}
