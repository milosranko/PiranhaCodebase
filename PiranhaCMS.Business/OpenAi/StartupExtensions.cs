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
        services
            .AddOpenAIChatCompletion("gpt-3.5-turbo-16k", config.GetRequiredSection(OpenAiOptions.Position).Value, "org-BeYeyeOLo1Jc6sRTGzlHLqkO")
            .AddTransient<IOpenAiService, OpenAiService>();

        return services;
    }
}
