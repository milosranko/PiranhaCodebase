using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;
using PiranhaCMS.Validators.Services;
using PiranhaCMS.Validators.Services.Interfaces;
using System;

namespace PiranhaCMS.Validators.Startup
{
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
            ILogger logger)
        {
            var siteValidatorService = app.ApplicationServices.GetService<ISiteValidatorService>();
            var pageValidatorService = app.ApplicationServices.GetService<IPageValidatorService>();

            if (pageValidatorService == null && siteValidatorService == null) throw new Exception("Validator service not initialized!");

            if (pageValidatorService != null)
            { 
                pageValidatorService.Initialize();
                App.Hooks.Pages.RegisterOnBeforeSave(x => pageValidatorService.Validate(x, logger));
            }

            if (siteValidatorService != null)
            {
                siteValidatorService.Initialize();
                App.Hooks.SiteContent.RegisterOnBeforeSave(x => siteValidatorService.Validate(x, logger));
            }

            return app;
        }
    }
}
