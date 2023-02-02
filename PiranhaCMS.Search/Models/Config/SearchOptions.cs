using System;

namespace PiranhaCMS.Search.Models.Config;

internal class SearchOptions
{
    public SearchOptions(Type[] include)
    {
        Include = include;
    }

    internal static Type[] Include { get; private set; }
}
