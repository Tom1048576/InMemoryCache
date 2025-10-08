using Microsoft.Extensions.DependencyInjection;
using ServiceLogic.Interfaces;
using ServiceLogic.Services;

namespace ServiceLogic;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceLogic(this IServiceCollection services)
    {
        services.AddScoped<ISalesSnapshotService, SalesSnapshotService>();
        services.AddScoped<ISupportAnalyticsService, SupportAnalyticsService>();
        return services;
    }
}
