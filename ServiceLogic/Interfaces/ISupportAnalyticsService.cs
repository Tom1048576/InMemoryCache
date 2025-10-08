using System.Collections.Generic;
using ServiceLogic.Models;

namespace ServiceLogic.Interfaces;

public interface ISupportAnalyticsService
{
    Task<IReadOnlyDictionary<string, SupportTicketCount>> GetOpenTicketsByPriorityAsync(CancellationToken cancellationToken);
}
