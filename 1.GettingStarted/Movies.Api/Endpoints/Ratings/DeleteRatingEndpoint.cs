using Movies.Api.Auth;
using Movies.Application.Services;

namespace Movies.Api.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    public const string Name = "DeleteRating";

    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.DeleteRating, async (
            Guid id,
            IRatingService ratingService,
            HttpContext context,
            CancellationToken cToken) =>
        {
            var userId = context.GetUserId();
            var result = await ratingService.DeleteRatingAsync(id, userId!.Value, cToken);
            return result ? Results.Ok() : Results.NotFound();
        })
        .WithName(Name)
        .RequireAuthorization();

        return app;
    }
}
