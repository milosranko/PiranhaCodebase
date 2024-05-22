using PiranhaCMS.Business.OpenAi.Dto;
using System.Text.Json.Serialization;

namespace PiranhaCMS.Business.OpenAi.Models;

internal record ChatResponse
{
    public string Id { get; set; }
    [JsonPropertyName("_object")]
    public string ObjectName { get; set; }
    public int Created { get; set; }
    public string Model { get; set; }
    [JsonPropertyName("system_fingerprint")]
    public string SystemFingerprint { get; set; }
    public Choice[] Choices { get; set; }
    public Usage Usage { get; set; }

    public ResponseDto? ToDto()
    {
        if (Choices == null || Choices.Length == 0)
            return null;

        return new ResponseDto(Choices.First().Message.Content);
    }
}
