using Microsoft.Extensions.DependencyInjection;
using NDB.Audit.EF.Abstractions;
using NDB.Audit.EF.Internal;

namespace NDB.Audit.EF.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNdbAudit(
        this IServiceCollection services)
    {
        services.AddScoped<IAuditService, DefaultAuditService>();
        return services;
    }
}
