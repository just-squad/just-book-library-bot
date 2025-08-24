using PenumbraHerald.Services;
using Telegram.Bot.Polling;

namespace PenumbraHerald.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUpdateHandlers(this IServiceCollection services)
    {
        services.AddScoped<IUpdateHandler, UpdateHandler>();
        return services;
    }
}
