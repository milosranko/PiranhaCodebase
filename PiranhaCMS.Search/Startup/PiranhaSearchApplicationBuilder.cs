namespace PiranhaCMS.Search.Startup;

public class PiranhaSearchApplicationBuilder
{
    //public readonly IApplicationBuilder Builder;
    public bool ForceReindexing { get; set; }
    public bool UseTextHighlighter { get; set; }
    public bool UseFacets { get; set; }
    public Type[] Include { get; set; }

    //public PiranhaSearchApplicationBuilder(IApplicationBuilder builder) => this.Builder = builder;
}
