using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class InMemoryMovieRepository : IMovieRepository
{
    private readonly List<Movie> _movies = [];
    
    public Task<bool> CreateAsync(Movie movie, CancellationToken cToken)
    {
        _movies.Add(movie);
        return Task.FromResult(true);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cToken)
    {
        var movie = _movies.SingleOrDefault(x => x.Id == id);
        return Task.FromResult(movie);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cToken)
    {
        return Task.FromResult(_movies.AsEnumerable());
    }

    public Task<bool> UpdateAsync(Movie movie, CancellationToken cToken)
    {
        var movieIndex = _movies.FindIndex(x => x.Id == movie.Id);
        if (movieIndex == -1) return Task.FromResult(false);
        
        _movies[movieIndex] = movie;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken cToken)
    {
        var removedCount = _movies.RemoveAll(x => x.Id == id);
        var movieRemoved = removedCount > 0;
        return Task.FromResult(movieRemoved);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken cToken)
    {
        var movie = _movies.SingleOrDefault(x => x.Slug == slug);
        return Task.FromResult(movie);
    }

    public Task<bool> ExistsByIdAsync(Guid id, CancellationToken cToken) => 
        Task.FromResult(_movies.Any(x => x.Id == id));
}
