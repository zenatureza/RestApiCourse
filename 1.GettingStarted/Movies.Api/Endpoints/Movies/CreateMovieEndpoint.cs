using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create, async (
            [FromBody] CreateMovieRequest request,
            IMovieService movieService,
            IOutputCacheStore outputCacheStore,
            HttpContext context,
            CancellationToken cToken) =>
        {
            var movie = request.MapToMovie();
            await movieService.CreateAsync(movie, cToken);
            await outputCacheStore.EvictByTagAsync("movies", cToken);

            return TypedResults.CreatedAtRoute(
                movie.MapToResponse(),
                GetMovieEndpoint.Name,
                new { idOrSlug = movie.Id });
        })
        .WithName(Name)
        .Produces<MovieResponse>(StatusCodes.Status201Created)
        .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
