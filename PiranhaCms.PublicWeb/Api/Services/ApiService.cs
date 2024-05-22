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

    public async Task<string?> SendChatGptPrompt(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        var query = text.Trim();
        var cacheKey = CreateCacheKey(query);
        var cachedValue = _cache.Get<string>(cacheKey);

        if (string.IsNullOrEmpty(cachedValue))
        {
            var res = await _openAiService.CreatePrompt(new RequestDto(text));
            if (res is null || string.IsNullOrEmpty(res.Text))
                return null;

            cachedValue = res.Text;
            _cache.Set(cacheKey, cachedValue, TimeSpan.FromDays(1));
        }

        return cachedValue;
    }

    private static string CreateCacheKey(string query)
    {
        return $"{CACHE_KEY}{query.Replace(" ", "")}";
    }
}
