using GodLeaks.Application.Interfaces;
using GodLeaks.Infrastructure.LeakSightAPI;
using GodLeaks.Infrastructure.Persistence;
using GodLeaks.Infrastructure.Services;
using GodLeaks.Presentation.Console;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GodLeaks.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServerCtlServices(this IServiceCollection services)
    {
        services.AddHttpClient<ILeakSight, LeakSight>(config =>
        {
            config.BaseAddress = new Uri("https://api.leaksights.com/");
        });
        services.AddDbContext<GodLeaksDbContext>(options => options.UseSqlite("Data Source=godleaks.db"));
        services.AddScoped<IAdmin, Admin>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddSingleton<IConsoleUi, ConsoleUi>();

        return services;
    }
}