using System.Text.Json.Serialization;

namespace PiranhaCMS.Business.OpenAi.Models;

internal record Choice
{
    public int Index { get; set; }
    public Message Message { get; set; }
    public object Logprobs { get; set; }
    [JsonPropertyName("finish_reason")]
    public string FinishReason { get; set; }
}
