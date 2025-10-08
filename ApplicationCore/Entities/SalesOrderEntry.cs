using ApplicationCore.Attributes;

namespace ApplicationCore.Entities;

[TableMetadata("Sales", "dbo.SalesOrders")]
public sealed record SalesOrderEntry(
    string CustomerName,
    DateTime OrderDate,
    decimal TotalAmount);
