using ComplianceCrm.Application.Abstractions.Persistence;
using ComplianceCrm.Application.Abstractions.Providers;
using ComplianceCrm.Application.Abstractions.Services;
using ComplianceCrm.Infrastructure.Audit;
using ComplianceCrm.Infrastructure.Identity;
using ComplianceCrm.Infrastructure.Persistence;
using ComplianceCrm.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ComplianceCrm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing Postgres connection string.");

        //services.AddHttpContextAccessor();

        services.AddScoped<AuditStampInterceptor>();

        services.AddDbContext<AppDbContext>((sp, opt) =>
        {
            opt.UseNpgsql(conn, npg =>
            {
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            });

            opt.UseSnakeCaseNamingConvention();
            // Recommended defaults
            opt.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            opt.EnableSensitiveDataLogging(false);

            // Interceptors
            opt.AddInterceptors(sp.GetRequiredService<AuditStampInterceptor>());
        });

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        // Providers
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICorrelationIdProvider, CorrelationIdProvider>();
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        // Services
        services.AddScoped<IBusinessAuditService, BusinessAuditService>();

        return services;
    }
}
