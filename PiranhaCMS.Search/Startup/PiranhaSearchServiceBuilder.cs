using Microsoft.Extensions.DependencyInjection;
using PiranhaCMS.Search.Models.Enums;

namespace PiranhaCMS.Search.Startup;

public class PiranhaSearchServiceBuilder
{
	public readonly IServiceCollection Services;
	public IndexDirectory StorageType { get; set; }
	public string IndexDirectory { get; set; }
	public string AzureStorageCredentials { get; set; }
	public DefaultAnalyzer DefaultAnalyzer { get; set; }

	public PiranhaSearchServiceBuilder(IServiceCollection services) => this.Services = services;
}
