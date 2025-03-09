using Movies.Application.Models;

namespace Movies.Application.Services;

public interface IMovieService
{
    Task<bool> CreateAsync(Movie movie, CancellationToken cToken = default);
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cToken = default);
    Task<Movie?> GetBySlugAsync(string slug, CancellationToken cToken = default);
    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cToken = default);
    Task<Movie?> UpdateAsync(Movie movie, CancellationToken cToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cToken = default);
}
