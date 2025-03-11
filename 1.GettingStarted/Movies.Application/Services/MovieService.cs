using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;
    private readonly IValidator<Movie> _validator;

    public MovieService(IMovieRepository repository, IValidator<Movie> validator)
    {
        _repository = repository;
        _validator = validator;
    }
    public async Task<bool> CreateAsync(Movie movie, CancellationToken cToken)
    {
        await _validator.ValidateAndThrowAsync(movie, cToken);
        return await _repository.CreateAsync(movie, cToken);
    }
    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken cToken = default)
    {
        return await _repository.GetByIdAsync(id, userId, cToken);
    }
    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cToken = default)
    {
        return await _repository.GetBySlugAsync(slug, userId, cToken);
    }
    public async Task<IEnumerable<Movie>> GetAllAsync(Guid? userId = default, CancellationToken cToken = default)
    {
        return await _repository.GetAllAsync(userId, cToken);
    }
    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken cToken = default)
    {
        await _validator.ValidateAndThrowAsync(movie);
        var movieExists = await _repository.ExistsByIdAsync(movie.Id, cToken);
        if (!movieExists) return null;

        await _repository.UpdateAsync(movie, userId, cToken);
        return movie;
    }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cToken)
    {
        return await _repository.DeleteAsync(id, cToken);
    }
}