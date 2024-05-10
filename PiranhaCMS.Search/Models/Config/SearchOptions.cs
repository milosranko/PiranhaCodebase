namespace PiranhaCMS.Search.Models.Config;

internal record SearchOptions
{
    public SearchOptions(Type[] include)
    {
        Include = include;
    }

    internal static Type[] Include { get; private set; }
}
