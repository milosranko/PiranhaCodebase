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
        //var options = config.GetSection(OpenAiOptions.Position).Get<OpenAiOptions>();
        var options = config.GetSection(AzureOpenAiOptions.Position).Get<AzureOpenAiOptions>();

        if (options is null || string.IsNullOrEmpty(options.ApiKey))
            throw new ArgumentNullException(nameof(options));

        services
            .AddAzureOpenAIChatCompletion(options.DeploymentName, options.Endpoint, options.ApiKey)
            //.AddOpenAIChatCompletion("gpt-3.5-turbo", options.ApiKey, options.OrganisationId)
            //.AddTransient<IOpenAiService, OpenAiService>()
            .AddTransient<IOpenAiService, AzureOpenAiService>();

        return services;
    }
}
