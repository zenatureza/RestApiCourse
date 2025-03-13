namespace Movies.Application.Repositories;

public interface IRatingRepository
{
    Task<float?> GetRatingAsync(Guid movieId, CancellationToken cToken = default);
    Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cToken = default);
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cToken = default);
}
