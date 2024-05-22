using RestSharp;

namespace PiranhaCMS.Business.OpenAi.Abstractions;

internal interface IOpenApiClient : IRestClient
{
    RestRequest GetChatRequest();
}
