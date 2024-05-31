using Piranha.Models;
using System.Reflection;

namespace PiranhaCMS.Validators.Services.Interfaces;

public interface ISiteValidatorService
{
    void Initialize(Assembly modelsAssembly);
    void Validate(SiteContentBase model);
}
