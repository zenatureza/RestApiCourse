using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";

    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Ratings.GetUserRatings, async (
            Guid id,
            IRatingService ratingService,
            HttpContext context,
            CancellationToken cToken) =>
        {
            var userId = context.GetUserId();
            var ratings = await ratingService.GetRatingsForUserAsync(userId!.Value, cToken);
            var response = ratings.MapToResponse();

            return TypedResults.Ok(response);
        })
        .WithName(Name)
        .RequireAuthorization();

        return app;
    }
}