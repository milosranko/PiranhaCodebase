using Microsoft.Extensions.Logging;
using Piranha.Models;

namespace PiranhaCMS.Validators.Services.Interfaces;

public interface IPageValidatorService
{
    void Initialize();
    void Validate(PageBase model, ILogger logger);
}
