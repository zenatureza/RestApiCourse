using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken cToken = default);
    Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken cToken = default);
    Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cToken = default);
    Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOption options, CancellationToken cToken = default);
    Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken cToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cToken = default);
    Task<int> CountAsync(string? title, int? yearOfRelease, CancellationToken cToken = default);
}
