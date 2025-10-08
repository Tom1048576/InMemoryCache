using ApplicationCore.Entities;
using Infrastructure.Factories;
using Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.DataLoaders;

public sealed class SupportTicketsTableLoader : DapperTableDataLoader<SupportTicketEntry>
{
    public SupportTicketsTableLoader(
        IDatabaseConnectionFactory connectionFactory,
        IOptions<DatabaseFleetOptions> options,
        ILogger<SupportTicketsTableLoader> logger)
        : base("SupportTickets", connectionFactory, options, logger)
    {
    }
}
