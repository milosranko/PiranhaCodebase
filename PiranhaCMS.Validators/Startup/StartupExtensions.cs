using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using PiranhaCMS.Validators.Services;
using PiranhaCMS.Validators.Services.Interfaces;
using System.Reflection;

namespace PiranhaCMS.Validators.Startup;

public static class StartupExtensions
{
    public static IServiceCollection AddPiranhaValidators(
        this IServiceCollection services,
        Action<PiranhaValidatorsServiceBuilder> options)
    {
        var serviceBuilder = new PiranhaValidatorsServiceBuilder(services);
        options?.Invoke(serviceBuilder);

        App.Modules.Register<Module>();

        if (serviceBuilder.UseSiteValidation)
            serviceBuilder.Services.AddSingleton<ISiteValidatorService, SiteValidatorService>();

        if (serviceBuilder.UsePageValidation)
            serviceBuilder.Services.AddSingleton<IPageValidatorService, PageValidatorService>();

        return serviceBuilder.Services;
    }

    public static IApplicationBuilder UsePiranhaValidators(
        this IApplicationBuilder app,
        Assembly modelsAssembly)
    {
        var siteValidatorService = app.ApplicationServices.GetService<ISiteValidatorService>();
        var pageValidatorService = app.ApplicationServices.GetService<IPageValidatorService>();

        if (pageValidatorService == null && siteValidatorService == null)
            throw new Exception("Validator service not initialized!");

        if (pageValidatorService != null)
        {
            pageValidatorService.Initialize(modelsAssembly);
            App.Hooks.Pages.RegisterOnBeforeSave(x => pageValidatorService.Validate(x));
        }

        if (siteValidatorService != null)
        {
            siteValidatorService.Initialize(modelsAssembly);
            App.Hooks.SiteContent.RegisterOnBeforeSave(x => siteValidatorService.Validate(x));
        }

        return app;
    }
}
