namespace PiranhaCMS.Business.OpenAi.Abstractions;

public record OpenAiOptions
{
    public const string Position = "OpenAI:ApiKey";

    public string ApiKey { get; set; } = string.Empty;
}
