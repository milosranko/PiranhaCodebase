namespace PiranhaCMS.Business.OpenAi.Abstractions;

public record AzureOpenAiOptions
{
    public const string Position = "AzureOpenAI";
    public string ApiKey { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = string.Empty;
}
