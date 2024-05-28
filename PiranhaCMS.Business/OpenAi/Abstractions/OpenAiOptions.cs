namespace PiranhaCMS.Business.OpenAi.Abstractions;

public record OpenAiOptions
{
    public const string Position = "OpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string OrganisationId { get; set; } = string.Empty;
    public string AssistantId { get; set; } = string.Empty;
    public string FileId { get; set; } = string.Empty;
}
