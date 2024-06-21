using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PiranhaCMS.Business.OpenAi.Abstractions;
using PiranhaCMS.Business.OpenAi.Dto;
using PiranhaCMS.Common.Extensions;
using System.Text;

namespace PiranhaCMS.Business.OpenAi.Services;

internal class AzureOpenAiService(ILogger<AzureOpenAiService> log, IChatCompletionService chatService, IOptions<AzureOpenAiOptions> options) : IOpenAiService
{
    private readonly AzureOpenAiOptions _options = options.Value;
    private readonly ILogger<AzureOpenAiService> _log = log;
    private readonly IChatCompletionService _chatService = chatService;
    private static readonly string systemMessage = "You are a music expert with knowledge of music and artists from year 1930 up until now.";
    private static readonly string artistPromptTemplate1 = "Tell me about: \"{0}\". Response should be in one sentence.";
    private static readonly string artistPromptTemplate2 =
        @"Suggest three artists similar to: ""{0}"".
        Response should show just bulleted list with each artist in a new line, without leading text. If you can't find any related artist, respond with an empty string.
        Response example: 
        - The Rolling Stones
        - The Animals
        - The Kinks";
    private static readonly string releasePromptTemplate1 = "Tell me about release: \"{0}\".";
    private static readonly string releasePromptTemplate2 =
        @"Suggest three similar releases by other artists.
        Response should show just bulleted list with each release in a new line, without leading text. If you can't find any related releases, respond with an empty string.
        Response example: 
        - ""Are You Experienced?"" by ""Jimi Hendrix Experience""
        - ""Beggars Banquet"" by ""The Rolling Stones""
        - ""Village Green Preservation Society"" by ""The Kinks""";

    public async Task<ResponseDto?> CreateArtistPrompt(RequestDto request)
    {
        if (string.IsNullOrEmpty(request.Text))
            return null;

        var chatHistory = new ChatHistory(systemMessage);
        var responseSb = new StringBuilder();

        try
        {
            await InvokeAgentAsync(string.Format(artistPromptTemplate1, request.Text));
            await InvokeAgentAsync(string.Format(artistPromptTemplate2, request.Text), true);

            async Task InvokeAgentAsync(string input, bool parse = false)
            {
                //if (parse)
                //{
                //    //chatHistory.Add(new ChatMessageContent(AuthorRole.Tool, $"When creating an answer, always use artists from artists.json file."));
                //    //Adding an response example improves chances that real response will be constructed in a same way
                //    chatHistory.Add(new ChatMessageContent(AuthorRole.User, "Suggest three artists similar to The Beatles."));
                //    chatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, "- The Rolling Stones\n- The Animals\n- The Kinks"));
                //}
                chatHistory.Add(new ChatMessageContent(AuthorRole.User, input));

                var res = await _chatService.GetChatMessageContentAsync(chatHistory);
                if (parse && !string.IsNullOrEmpty(res.Content))
                    responseSb.Append($"Related artists:</br>{ParseSuggestedArtists(res.Content)}</br>");
                else
                    responseSb.Append($"{res.Content}</br>");
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Prompt value: {request.Text}");
        }

        return new ResponseDto(responseSb.ToString());
    }

    public async Task<ResponseDto?> CreateReleasePrompt(RequestDto request)
    {
        if (string.IsNullOrEmpty(request.Text))
            return null;

        var chatHistory = new ChatHistory(systemMessage);
        var responseSb = new StringBuilder();

        try
        {
            await InvokeAgentAsync(string.Format(releasePromptTemplate1, request.Text));
            await InvokeAgentAsync(releasePromptTemplate2, true);

            async Task InvokeAgentAsync(string input, bool parse = false)
            {
                //if (parse)
                //{
                //    chatHistory.Add(new ChatMessageContent(AuthorRole.User, "Suggest three similar releses to The Beatles White Album."));
                //    chatHistory.Add(new ChatMessageContent(AuthorRole.Assistant, "- Are You Experienced?\n- Beggars Banquet\n- Village Green Preservation Society"));
                //}
                chatHistory.Add(new ChatMessageContent(AuthorRole.User, input));

                var res = await _chatService.GetChatMessageContentAsync(chatHistory);
                if (parse && !string.IsNullOrEmpty(res.Content))
                    responseSb.Append($"Related releases:</br>{ParseSuggestedArtists(res.Content)}</br>");
                else
                    responseSb.Append($"{res.Content}</br>");
            }
        }
        catch (Exception ex)
        {
            _log.LogError(ex, $"Prompt value: {request.Text}");
        }

        return new ResponseDto(responseSb.ToString());
    }

    private static string ParseSuggestedArtists(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var artists = input
            .Split("\n")
            .Select(x => "<a href='?q=" + x.Remove(0, 2).Replace("by", "", StringComparison.InvariantCultureIgnoreCase).Replace("the", "", StringComparison.InvariantCultureIgnoreCase).SanitizeSearchString() + "'>" + x + "</a></br>")
            .ToArray();

        return string.Join("\n", artists);
    }
}
