using ApplicationCore.Attributes;

namespace ApplicationCore.Entities;

[TableMetadata("Support", "dbo.SupportTickets")]
public sealed record SupportTicketEntry(
    string TicketNumber,
    string? Priority,
    string? Status,
    DateTime UpdatedOn);
