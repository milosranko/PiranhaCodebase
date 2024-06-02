using Microsoft.Extensions.Caching.Memory;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Dto;

namespace PiranhaCMS.PublicWeb.Api.Services;

internal class ApiService : IApiService
{
    private readonly IOpenAiService _openAiService;
    private readonly IMemoryCache _cache;
    private const string CACHE_KEY = "key:";

    public ApiService(IOpenAiService openAiService, IMemoryCache cache)
    {
        _openAiService = openAiService;
        _cache = cache;
    }

    public async Task<string?> SendChatGptPrompt(string artist, string? release)
    {
        if (string.IsNullOrEmpty(artist))
            return null;

        if (string.IsNullOrEmpty(release))
        {
            var artistQuery = artist.Trim();
            var artistCacheKey = CreateCacheKey($"art:{artistQuery}");

            return _cache.Get<string>(artistCacheKey) ?? await CreatePrompt(artistCacheKey, artistQuery, false);
        }

        var releaseQuery = $"{release.Trim()} by {artist.Trim()}";
        var releaseCacheKey = CreateCacheKey($"art:{artist}:rel:{release}");

        return _cache.Get<string>(releaseCacheKey) ?? await CreatePrompt(releaseCacheKey, releaseQuery, true);
    }

    private async Task<string> CreatePrompt(string cacheKey, string query, bool release)
    {
        var res = release
            ? await _openAiService.CreateReleasePrompt(new RequestDto(query))
            : await _openAiService.CreateArtistPrompt(new RequestDto(query));

        if (res is null || string.IsNullOrEmpty(res.Text))
            return string.Empty;

        var cachedValue = res.Text;
        _cache.Set(cacheKey, cachedValue, TimeSpan.FromDays(1));

        return cachedValue;
    }

    private static string CreateCacheKey(string query)
    {
        return $"{CACHE_KEY}{query.Trim().Replace(" ", "")}";
    }
}
