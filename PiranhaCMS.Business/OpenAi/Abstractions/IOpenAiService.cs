using PiranhaCMS.Business.OpenAi.Dto;

namespace PiranhaCMS.Business.OpenAi.Abstractions;

public interface IOpenAiService
{
    Task<ResponseDto?> CreateArtistPrompt(RequestDto request);
    Task<ResponseDto?> CreateReleasePrompt(RequestDto request);
}
