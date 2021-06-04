using Piranha.Extend;

namespace PiranhaCMS.Search
{
    public class Module : IModule
    {
        public string Author => "Milos Rankovic";

        public string Name => "PiranhaCMS.Search";

        public string Version => Piranha.Utils.GetAssemblyVersion(this.GetType().Assembly);

        public string Description => "Search module for Piranha CMS using Lucene engine.";

        public string PackageUrl => "#";

        public string IconUrl => "https://piranhacms.org/assets/twitter-shield.png";

        public void Init() 
        { }
    }
}
