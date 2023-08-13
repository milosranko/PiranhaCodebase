using Microsoft.Extensions.Logging;
using Piranha.Models;
using System.Reflection;

namespace PiranhaCMS.Validators.Services.Interfaces;

public interface IPageValidatorService
{
    void Initialize(Assembly modelsAssembly);
    void Validate(PageBase model, ILogger logger);
}
