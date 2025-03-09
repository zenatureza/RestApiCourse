using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie, CancellationToken cToken = default);
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cToken = default);
    Task<Movie?> GetBySlugAsync(string slug, CancellationToken cToken = default);
    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cToken = default);
    Task<bool> UpdateAsync(Movie movie, CancellationToken cToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cToken = default);
}