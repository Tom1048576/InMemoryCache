using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using ServiceLogic;
using ServiceLogic.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddServiceLogic();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/cache/refresh", async (
    [FromServices] ITableCache tableCache,
    CancellationToken cancellationToken) =>
    {
        await tableCache.RefreshAllAsync(cancellationToken);
        return Results.Accepted();
    })
    .WithName("RefreshAllTables")
    .WithOpenApi(operation =>
    {
        operation.Summary = "Refresh every registered table cache immediately.";
        return operation;
    });

app.MapGet("/tables/sales-orders", async (
    [FromServices] ITableCache tableCache,
    CancellationToken cancellationToken) =>
    {
        var entries = await tableCache.GetAsync<SalesOrderEntry>(cancellationToken);
        return Results.Ok(entries);
    })
    .WithName("GetSalesOrderEntries")
    .WithOpenApi(operation =>
    {
        operation.Summary = "Return the cached sales order entries.";
        return operation;
    });

app.MapGet("/tables/support-tickets", async (
    [FromServices] ITableCache tableCache,
    CancellationToken cancellationToken) =>
    {
        var entries = await tableCache.GetAsync<SupportTicketEntry>(cancellationToken);
        return Results.Ok(entries);
    })
    .WithName("GetSupportTicketEntries")
    .WithOpenApi(operation =>
    {
        operation.Summary = "Return the cached support ticket entries.";
        return operation;
    });

app.MapGet("/sales/recent", async (
    [FromQuery] int? top,
    [FromServices] ISalesSnapshotService salesService,
    CancellationToken cancellationToken) =>
    {
        var results = await salesService.GetRecentOrdersAsync(top ?? 10, cancellationToken);
        return Results.Ok(results);
    })
    .WithName("GetRecentSalesOrders")
    .WithOpenApi(operation =>
    {
        operation.Summary = "Sample LINQ query over typed cache: recent sales orders.";
        return operation;
    });

app.MapGet("/support/open-by-priority", async (
    [FromServices] ISupportAnalyticsService supportService,
    CancellationToken cancellationToken) =>
    {
        var results = await supportService.GetOpenTicketsByPriorityAsync(cancellationToken);
        return Results.Ok(results);
    })
    .WithName("GetSupportTicketsByPriority")
    .WithOpenApi(operation =>
    {
        operation.Summary = "Sample LINQ aggregation over typed cache: open tickets grouped by priority.";
        return operation;
    });

app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }))
    .WithName("HealthCheck")
    .WithOpenApi(operation =>
    {
        operation.Summary = "Quick health check endpoint.";
        return operation;
    });

app.Run();
