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

    private static async Task<IResult> Get([FromQuery] string query, IApiService apiService)
    {
        if (string.IsNullOrEmpty(query))
            return Results.Empty;

        //var response = "The Beatles were a British rock band formed in Liverpool in 1960, consisting of John Lennon, Paul McCartney, George Harrison, and Ringo Starr, known for their innovative songwriting, musicianship, and huge influence on popular music.";
        var res = await apiService.SendChatGptPrompt(query);

        if (string.IsNullOrEmpty(res))
            return Results.Empty;

        return Results.Ok(res);
    }
}
