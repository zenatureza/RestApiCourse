using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update, async (
            Guid id,
            [FromBody] UpdateMovieRequest request,
            IMovieService movieService,
            IOutputCacheStore outputCacheStore,
            HttpContext context,
            CancellationToken cToken) =>
        {
            var movie = request.MapToMovie(id);
            var userId = context.GetUserId();
            var updatedMovie = await movieService.UpdateAsync(movie, userId, cToken);
            if (updatedMovie is null) return Results.NotFound();

            await outputCacheStore.EvictByTagAsync("movies", cToken);
            var response = movie.MapToResponse();
            return TypedResults.Ok(response);
        })
        .WithName(Name)
        .Produces<MoviesResponse>(StatusCodes.Status200OK)
        .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
