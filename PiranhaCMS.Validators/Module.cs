using Piranha.Extend;

namespace PiranhaCMS.Validators;

public class Module : IModule
{
	public string Author => "Milos Rankovic";
	public string Name => "MdrSolutions.Piranha.Validators";
	public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);
	public string Description => "Validators for site and page content";
	public string PackageUrl => "https://github.com/milosranko/PiranhaCodebase/tree/main/PiranhaCMS.Validators";
	public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

	public void Init()
	{ }
}
