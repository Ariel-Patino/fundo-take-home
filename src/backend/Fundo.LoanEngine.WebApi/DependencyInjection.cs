using Fundo.LoanEngine.Application.Applications.SubmitApplication;
using Fundo.LoanEngine.Application.Interfaces;
using Fundo.LoanEngine.Application.Rules;
using Fundo.LoanEngine.Infrastructure.External;
using Fundo.LoanEngine.Infrastructure.Outbox;
using Fundo.LoanEngine.Infrastructure.Persistence;
using Fundo.LoanEngine.Infrastructure.Persistence.Repositories;
using Fundo.LoanEngine.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;

namespace Fundo.LoanEngine.WebApi;

public static class DependencyInjection
{
    public static IServiceCollection AddLoanEngine(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());
        services.AddSingleton<IClock, SystemClock>();

        services.AddSingleton<IDenyRule>(
            new StateDenyRule(configuration.GetSection("Rules:BlockedStates").Get<string[]>() ?? ["NY"]));
        services.AddSingleton<IDenyRule>(
            new SsnBlacklistRule(configuration.GetSection("Rules:BlacklistedSsns").Get<string[]>() ?? []));
        services.AddScoped<RuleEngine>();
        services.AddScoped<SubmitApplicationValidator>();
        services.AddScoped<SubmitApplicationHandler>();
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<OutboxPublisher>();
        services.AddHostedService<OutboxBackgroundService>();

        services.AddHttpClient<IExternalMockClient, ExternalMockClient>(client =>
        {
            var baseUrl = configuration["ExternalMockService:BaseUrl"]
                ?? throw new InvalidOperationException("External mock service base URL is not configured.");
            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}