using Microsoft.AspNetCore.Mvc;
using PiranhaCMS.PublicWeb.Api.Services;

namespace PiranhaCMS.PublicWeb.Api;

public static class Endpoints
{
    public static IEndpointRouteBuilder UseApiEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/api");
        group.MapGet("/chatgpt", Get)
            .Produces<string>();
        //.RequireAuthorization();

        return builder;
    }

    private static async Task<IResult> Get([FromQuery] string q, IApiService apiService)
    {
        if (string.IsNullOrEmpty(q))
            return Results.Empty;

        var res = await apiService.SendChatGptPrompt(q);

        if (string.IsNullOrEmpty(res))
            return Results.Empty;

        return Results.Ok(res);
    }
}
