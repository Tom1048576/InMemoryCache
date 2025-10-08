using ApplicationCore.Entities;
using Infrastructure.Factories;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataLoaders;

public sealed class SalesOrdersTableLoader : DapperTableDataLoader<SalesOrderEntry>
{
    public SalesOrdersTableLoader(
        IDatabaseConnectionFactory connectionFactory,
        IOptions<DatabaseFleetOptions> options,
        ILogger<SalesOrdersTableLoader> logger)
        : base("SalesOrders", connectionFactory, options, logger)
    {
    }
}
