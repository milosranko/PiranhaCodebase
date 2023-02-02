using Microsoft.Extensions.DependencyInjection;

namespace PiranhaCMS.Validators.Startup;

public class PiranhaValidatorsServiceBuilder
{
    public readonly IServiceCollection Services;
    public bool UseSiteValidation { get; set; }
    public bool UsePageValidation { get; set; }
    public PiranhaValidatorsServiceBuilder(IServiceCollection services) => this.Services = services;
}
