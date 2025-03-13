using Movies.Application.Models;
using Movies.Contracts.Requests;
using Movies.Contracts.Responses;

namespace Movies.Api.Mapping;

public static class ContractMapping
{
    public static Movie MapToMovie(this CreateMovieRequest request) => new()
    {
        Id = Guid.NewGuid(),
        Title = request.Title,
        YearOfRelease = request.YearOfRelease,
        Genres = [.. request.Genres]
    };

    public static Movie MapToMovie(this UpdateMovieRequest request, Guid id) => new()
    {
        Id = id,
        Title = request.Title,
        YearOfRelease = request.YearOfRelease,
        Genres = [.. request.Genres]
    };

    public static MovieResponse MapToResponse(this Movie movie) => new()
    {
        Id = movie.Id,
        Title = movie.Title,
        Slug = movie.Slug,
        YearOfRelease = movie.YearOfRelease,
        Genres = movie.Genres,
        Rating = movie.Rating,
        UserRating = movie.UserRating
    };

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies) =>
        new()
        {
            Items = movies.Select(MapToResponse)
        };

    public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> movieRatings)
    {
        return movieRatings.Select(x => new MovieRatingResponse
        {
            MovieId = x.MovieId,
            Slug = x.Slug,
            Rating = x.Rating
        });
    }
}
