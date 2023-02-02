using Microsoft.Extensions.Logging;
using Piranha.Models;

namespace PiranhaCMS.Validators.Services.Interfaces;

public interface ISiteValidatorService
{
    void Initialize();
    void Validate(SiteContentBase model, ILogger logger);
}
