using Microsoft.Extensions.Caching.Memory;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Dto;

namespace PiranhaCMS.PublicWeb.Api.Services;

internal class ApiService : IApiService
{
    #region Private fields

    private readonly IOpenAiService _openAiService;
    private readonly IMemoryCache _cache;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<ApiService> _logger;
    private const string CACHE_KEY = "key:";

    #endregion

    #region Constructors

    public ApiService(
        IOpenAiService openAiService,
        IMemoryCache cache,
        IHttpContextAccessor contextAccessor,
        ILogger<ApiService> logger)
    {
        _openAiService = openAiService;
        _cache = cache;
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    #endregion

    #region Public methods

    public async Task<string?> SendChatGptPrompt(string artist, string? release)
    {
        _logger.LogInformation("ChatGPT request with params: artist: {0}, release: {1}", artist, release);

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

    #endregion

    #region Private methods

    private async Task<string> CreatePrompt(string cacheKey, string query, bool release)
    {
        if (!HasApiAccess())
            return "You have used up your ChatGPT daily limit!";

        var res = release
            ? await _openAiService.CreateReleasePrompt(new RequestDto(query))
            : await _openAiService.CreateArtistPrompt(new RequestDto(query));

        if (res is null || string.IsNullOrEmpty(res.Text))
            return string.Empty;

        var cachedValue = res.Text;
        _ = _cache.Set(cacheKey, cachedValue, TimeSpan.FromDays(1));

        return cachedValue;
    }

    private bool HasApiAccess()
    {
        if (_contextAccessor.HttpContext is null || _contextAccessor.HttpContext.Connection.RemoteIpAddress is null)
            return false;

        _logger.LogInformation("ChatGPT request from IP: {0}", _contextAccessor.HttpContext.Connection.RemoteIpAddress);

        var cacheKey = CreateCacheKey($"ip:{_contextAccessor.HttpContext.Connection.RemoteIpAddress}");
        var cachedValue = _cache.Get<int>(cacheKey);

        if (cachedValue.Equals(0))
        {
            _ = _cache.Set<int>(cacheKey, 1, TimeSpan.FromDays(1));
            return true;
        }
        else if (cachedValue.Equals(1))
        {
            _ = _cache.Set<int>(cacheKey, 2, TimeSpan.FromDays(1));
            return true;
        }

        return false;
    }

    private static string CreateCacheKey(string query)
    {
        return $"{CACHE_KEY}{query.Trim().Replace(" ", "")}";
    }

    #endregion
}
