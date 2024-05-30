using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Dto;
using System.Text;

namespace PiranhaCMS.Business.OpenAi.Services;

internal class OpenAiService(ILogger<OpenAiService> log, IChatCompletionService chatService, IOptions<OpenAiOptions> options) : IOpenAiService
{
    private readonly OpenAiOptions _options = options.Value;
    private readonly ILogger<OpenAiService> _log = log;
    private readonly IChatCompletionService _chatService = chatService;
    private static readonly string promptTemplate1 = "Tell me about: {0}.";
    private static readonly string promptTemplate2 =
        @"Suggest three similar artists. Artists are contained in artists.txt file.
        Response should show just bulleted list with each artist in a new line, without leading text. If you can't find any related artist, respond with an empty string.";

    public async Task<ResponseDto?> CreatePrompt(RequestDto request)
    {
        if (string.IsNullOrEmpty(request.Text))
            return null;

#pragma warning disable SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        var agent = await OpenAIAssistantAgent.RetrieveAsync(new(), new(_options.ApiKey), _options.AssistantId);
        var chat = new AgentGroupChat(agent);
        var responseSb = new StringBuilder();

        try
        {
            await InvokeAgentAsync(string.Format(promptTemplate1, request.Text));
            await InvokeAgentAsync(promptTemplate2, true);

            async Task InvokeAgentAsync(string input, bool parse = false)
            {
                if (parse)
                {
                    chat.AddChatMessage(new ChatMessageContent(AuthorRole.Tool, $"artists.txt file_id: {_options.FileId}"));
                    //Adding an response example improves chances that real response will be constructed in a same way
                    chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, "Suggest three similar artists."));
                    chat.AddChatMessage(new ChatMessageContent(AuthorRole.Assistant, "- Artist 1\n- Artist 2\n- Artist 3"));
                }
                chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, input));
                await foreach (var content in chat.InvokeAsync(agent))
                {
                    if (parse && !string.IsNullOrEmpty(content.Content))
                        responseSb.AppendLine($"Related artists:</br>{ParseSuggestedArtists(content.Content)}</br>");
                    else
                        responseSb.AppendLine($"{content.Content}</br>");
                }
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Prompt value: {request.Text}");
        }

        return new ResponseDto(responseSb.ToString());
#pragma warning restore SKEXP0110 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        //var chatHistory = new ChatHistory(
        //@"You are a music expert with knowledge of music and artists from year 1920 up until now.
        //Response should be in one sentence, up to 400 characters long.");
        //chatHistory.AddUserMessage($"Tell me about: {request.Text}.");

        //try
        //{
        //    var res = await _chatService.GetChatMessageContentAsync(chatHistory);

        //    if (res is null || string.IsNullOrEmpty(res.Content))
        //        return new ResponseDto(string.Empty);

        //    chatHistory.AddAssistantMessage(res.Content);
        //    var response = $"{res.Content}</br>Also checkout these artists:</br>";
        //    chatHistory.AddUserMessage(
        //        @"Suggest three similar artists. Artists suggested must be contained in vector store: vs_9fWsxolVklyZrk4LBZ5H1Lns. 
        //        Response should show bulleted list with each artist in a new line.");

        //    res = await _chatService.GetChatMessageContentAsync(chatHistory);

        //    if (res != null)
        //        return new ResponseDto($"{response}{ParseSuggestedArtists(res.Content)}");
        //}
        //catch (Exception ex)
        //{
        //    _log.LogError(ex, $"Prompt values: {string.Join(Environment.NewLine, chatHistory.Select(x => x.Content))}");
        //}

        //return null;
    }

    private string ParseSuggestedArtists(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var artists = input
            .Split("\n")
            .Select(x => "<a href='?q=" + x.Remove(0, 2) + "'>" + x + "</a></br>")
            .ToArray();

        return string.Join("\n", artists);
    }
}
