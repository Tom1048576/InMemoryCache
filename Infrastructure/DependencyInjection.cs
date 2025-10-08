using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Caching;
using Infrastructure.DataLoaders;
using Infrastructure.Factories;
using Infrastructure.HostedServices;
using Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseFleetOptions>(configuration.GetSection(DatabaseFleetOptions.SectionName));
        services.Configure<CacheRefreshOptions>(configuration.GetSection(CacheRefreshOptions.SectionName));

        services.AddSingleton<IDatabaseConnectionFactory, SqlConnectionFactory>();

        services.AddSingleton<ITableDataLoader<SalesOrderEntry>, SalesOrdersTableLoader>();
        services.AddSingleton<ITableDataLoader<SupportTicketEntry>, SupportTicketsTableLoader>();
        services.AddSingleton<ITableDataLoader>(sp => sp.GetRequiredService<ITableDataLoader<SalesOrderEntry>>());
        services.AddSingleton<ITableDataLoader>(sp => sp.GetRequiredService<ITableDataLoader<SupportTicketEntry>>());

        services.AddSingleton<ITableCache, TableCache>();
        services.AddHostedService<TableCacheRefreshService>();

        return services;
    }
}
