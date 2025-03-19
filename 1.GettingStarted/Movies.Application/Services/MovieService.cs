using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;
using Movies.Application.Validators;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _repository;
    private readonly IValidator<Movie> _validator;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<GetAllMoviesOption> _optionsValidator;

    public MovieService(
        IMovieRepository repository,
        IValidator<Movie> validator,
        IRatingRepository ratingRepository,
        IValidator<GetAllMoviesOption> optionsValidator)
    {
        _repository = repository;
        _validator = validator;
        _ratingRepository = ratingRepository;
        _optionsValidator = optionsValidator;
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

    // TODO: receber GetAllMoviesOption
    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOption options, CancellationToken cToken = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, cToken);

        return await _repository.GetAllAsync(options, cToken);
    }
    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken cToken = default)
    {
        await _validator.ValidateAndThrowAsync(movie);
        var movieExists = await _repository.ExistsByIdAsync(movie.Id, cToken);
        if (!movieExists) return null;

        await _repository.UpdateAsync(movie, cToken);
        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, cToken);
            movie.Rating = rating;
            return movie;
        }

        var (Rating, UserRating) = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, cToken);
        movie.Rating = Rating;
        movie.UserRating = UserRating;
        return movie;
    }
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cToken)
    {
        return await _repository.DeleteAsync(id, cToken);
    }
}