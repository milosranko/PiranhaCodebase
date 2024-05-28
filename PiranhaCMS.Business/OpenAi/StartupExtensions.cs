using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Services;

namespace PiranhaCMS.Business.OpenAi;

public static class StartupExtensions
{
    public static IServiceCollection AddOpenAiApi(this IServiceCollection services, IConfiguration config)
    {
        var options = config.GetSection(OpenAiOptions.Position).Get<OpenAiOptions>();

        if (options is null || string.IsNullOrEmpty(options.ApiKey))
            throw new ArgumentNullException(nameof(options));

        services
            .AddOpenAIChatCompletion("gpt-3.5-turbo", options.ApiKey, options.OrganisationId)
            .AddTransient<IOpenAiService, OpenAiService>();

        return services;
    }
}
