using Microsoft.Extensions.Logging;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Dto;
using PiranhaCMS.Business.OpenAi.Models;
using RestSharp;

namespace PiranhaCMS.Business.OpenAi.Services;

internal class OpenAiService(ILogger<OpenAiService> log, IOpenApiClient httpClient) : IOpenAiService
{
    private readonly IOpenApiClient _httpClient = httpClient;
    private readonly ILogger<OpenAiService> _log = log;

    public async Task<ResponseDto?> CreatePrompt(RequestDto request)
    {
        if (string.IsNullOrEmpty(request.Text))
            return null;

        var requestModel = ChatRequest.FromDto(request);

        var chatRequest = _httpClient.GetChatRequest();
        chatRequest.AddBody(requestModel, ContentType.Json);

        try
        {
            var restRes = await _httpClient.ExecuteAsync<ChatResponse>(chatRequest);

            if (restRes.IsSuccessStatusCode)
                return restRes.Data?.ToDto();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Prompt values: {string.Join(Environment.NewLine, requestModel.Messages.Select(x => x.ToString()))}");
        }

        return null;
    }
}
