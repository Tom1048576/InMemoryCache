using System.Collections.Generic;
using ServiceLogic.Models;

namespace ServiceLogic.Interfaces;

public interface ISalesSnapshotService
{
    Task<IReadOnlyList<SalesOrderSummary>> GetRecentOrdersAsync(int top, CancellationToken cancellationToken);
}
