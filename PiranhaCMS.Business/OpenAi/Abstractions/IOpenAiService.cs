using PiranhaCMS.Business.OpenAi.Dto;

namespace PiranhaCMS.Business.OpenAi.Abstractions;

public interface IOpenAiService
{
    Task<ResponseDto?> CreatePrompt(RequestDto request);
}
