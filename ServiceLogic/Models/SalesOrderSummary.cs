namespace ServiceLogic.Models;

public sealed record SalesOrderSummary(
    string CustomerName,
    DateTime OrderDate,
    decimal TotalAmount);
