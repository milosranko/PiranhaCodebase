namespace PiranhaCMS.PublicWeb.Api.Services;

public interface IApiService
{
    Task<string?> SendChatGptPrompt(string artist, string? release);
}
