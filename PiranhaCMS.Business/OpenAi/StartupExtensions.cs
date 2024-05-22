using Microsoft.Extensions.DependencyInjection;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Client;
using PiranhaCMS.Business.OpenAi.Services;

namespace PiranhaCMS.Business.OpenAi;

public static class StartupExtensions
{
    public static IServiceCollection AddOpenAiApi(this IServiceCollection services)
    {
        services
            .AddSingleton<IOpenApiClient, OpenAiApiClient>()
            .AddTransient<IOpenAiService, OpenAiService>();

        return services;
    }
}
