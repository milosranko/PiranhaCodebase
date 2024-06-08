using PiranhaCMS.Business.OpenAi.Dto;

namespace PiranhaCMS.Business.OpenAi.Models;

internal record ChatRequest
{
    public string Model => "gpt-3.5-turbo";
    public Message[] Messages { get; private set; }

    public static ChatRequest FromDto(RequestDto request)
    {
        var messages = new Message[2];
        messages[0] = new Message("system", "You are a music expert with knowledge of music and artists from 1920 up until now. Response should be in one sentence, up to 300 characters long.");
        messages[1] = new Message("user", $"Tell me about {request.Text}.");

        return new ChatRequest
        {
            Messages = messages
        };
    }
}
