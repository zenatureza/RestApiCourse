using Dapper;
using Movies.Application.Database;

namespace Movies.Application.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var result = await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            SELECT 
                ROUND(AVG(r.rating), 1)
            FROM ratings r
            WHERE r.movieId = @movieId
            """, new { movieId }, cancellationToken: cToken));

        return result;
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var result = await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            SELECT 
                ROUND(AVG(r.rating), 1),
                (SELECT rating FROM ratings WHERE movieId = @movieId AND userId = @userId LIMIT 1)
            FROM ratings r
            WHERE r.movieId = @movieId
            """, new { movieId }, cancellationToken: cToken));

        return result;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var result = await connection.ExecuteAsync(new CommandDefinition("""
            INSERT INTO ratings (movieId, userId, rating)
            VALUES (@movieId, @userId, @rating)
            ON CONFLICT (movieId, userId) DO UPDATE SET rating = @rating
            """, new { movieId, userId, rating }, cancellationToken: cToken));

        return result > 0;
    }
}