using Microsoft.Extensions.Options;
using PiranhaCMS.Business.OpenAi.Abstractions;
using RestSharp;

namespace PiranhaCMS.Business.OpenAi.Client;

internal class OpenAiApiClient : RestClient, IOpenApiClient
{
    private const string API_URL = "https://api.openai.com";
    private const string CHAT_ENDPOINT = "/v1/chat/completions";
    private readonly OpenAiOptions _options;

    public OpenAiApiClient(IOptions<OpenAiOptions> options) : base(new RestClientOptions(API_URL))
    {
        _options = options.Value;
    }

    public RestRequest GetChatRequest()
    {
        var request = new RestRequest(CHAT_ENDPOINT, Method.Post);
        request.AddHeader("Content-Type", ContentType.Json);
        request.AddHeader("Authorization", $"Bearer {_options.ApiKey}");

        return request;
    }
}
