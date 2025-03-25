﻿using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Endpoints.Movies;

public static class MapGetAllMoviesEndpoint
{
    public const string Name = "GetMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.GetAll, async (
            [AsParameters] GetAllMoviesRequest request,
            IMovieService movieService,
            HttpContext context,
            CancellationToken cToken) =>
        {
            var userId = context.GetUserId();
            var options = request.MapToOptions()
                .WithUserId(userId);

            var count = await movieService.CountAsync(options.Title, options.YearOfRelease, cToken);
            var movies = await movieService.GetAllAsync(options, cToken);
            return TypedResults.Ok(movies.MapToResponse(
                request.Page.GetValueOrDefault(PagedRequest.DefaultPage), 
                request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize), 
                count));
        }).WithName(Name);

        return app;
    }
}
