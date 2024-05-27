using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.ChatCompletion;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Dto;

namespace PiranhaCMS.Business.OpenAi.Services;

internal class OpenAiService(ILogger<OpenAiService> log, IChatCompletionService chatService) : IOpenAiService
{
    private readonly ILogger<OpenAiService> _log = log;
    private readonly IChatCompletionService _chatService = chatService;

    public async Task<ResponseDto?> CreatePrompt(RequestDto request)
    {
        if (string.IsNullOrEmpty(request.Text))
            return null;

        var chatHistory = new ChatHistory("You are a music expert with knowledge of music and artists from year 1920 up until now. Response should be in one sentence, up to 400 characters long.");
        chatHistory.AddUserMessage($"Tell me about {request.Text}.");

        try
        {
            var res = await _chatService.GetChatMessageContentAsync(chatHistory);

            if (res != null)
                return new ResponseDto(res.Content);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Prompt values: {string.Join(Environment.NewLine, chatHistory.Select(x => x.Content))}");
        }

        return null;
    }
}
