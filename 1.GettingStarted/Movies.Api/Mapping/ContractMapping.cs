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

    public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies, int page, int pageSize, int totalCount) =>
        new()
        {
            Items = movies.Select(MapToResponse),
            Page = page,
            PageSize = pageSize,
            Total = totalCount
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

    public static GetAllMoviesOption MapToOptions(this GetAllMoviesRequest request) => new()
    {
        YearOfRelease = request.YearOfRelease,
        Title = request.Title,
        SortField = request.SortBy?.Trim('-', '+').Trim(),
        SortOrder = request.SortBy is null ? SortOrder.Unsorted : 
            (request.SortBy.StartsWith('-') ? SortOrder.Descending : SortOrder.Ascending),
        Page = request.Page,
        PageSize = request.PageSize
    };

    public static GetAllMoviesOption WithUserId(this GetAllMoviesOption options, Guid? userId) 
    { 
        options.UserId = userId; 
        return options; 
    }
}
