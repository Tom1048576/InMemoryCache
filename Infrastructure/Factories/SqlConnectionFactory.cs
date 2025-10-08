using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Factories;

public interface IDatabaseConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(string connectionString, CancellationToken cancellationToken);
}

public sealed class SqlConnectionFactory : IDatabaseConnectionFactory
{
    public async Task<DbConnection> CreateOpenConnectionAsync(string connectionString, CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
