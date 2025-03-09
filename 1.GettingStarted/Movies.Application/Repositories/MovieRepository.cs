using Dapper;
using Movies.Application.Database;
using Movies.Application.Models;

namespace Movies.Application.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public MovieRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        using var transaction = connection.BeginTransaction();

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            
            INSERT INTO movies (id, slug, title, yearofrelease)
            VALUES (@Id, @Slug, @Title, @YearOfRelease);
            
            """, movie, cancellationToken: cToken));

        if (result > 0)
        {
            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition("""
                    INSERT INTO genres (movieId, name)
                    VALUES (@MovieId, @Name);
                    """, new { MovieId = movie.Id, Name = genre }, cancellationToken: cToken));
            }
        }
        transaction.Commit();

        return result > 0;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            new CommandDefinition("""
            SELECT *
            FROM movies
            WHERE id = @id;
            """, new { id }, cancellationToken: cToken));
        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
            SELECT name
            FROM genres
            WHERE movieId = @id;
            """, new { id }, cancellationToken: cToken));
        movie.Genres = [.. genres];

        return movie;
    }

    public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            new CommandDefinition("""
            SELECT *
            FROM movies
            WHERE slug = @slug;
            """, new { slug }, cancellationToken: cToken));
        if (movie is null) return null;

        var genres = await connection.QueryAsync<string>(
            new CommandDefinition("""
            SELECT name
            FROM genres
            WHERE movieId = @id;
            """, new { id = movie.Id }, cancellationToken: cToken));
        movie.Genres = [.. genres];

        return movie;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var result = await connection.QueryAsync(new CommandDefinition("""
            SELECT m.*, STRING_AGG(g.name, ',') AS genres
            FROM movies m LEFT JOIN genres g ON m.id = g.movieId
            GROUP BY m.id;
            """, cancellationToken: cToken));

        return result.Select(x => new Movie 
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            DELETE FROM genres WHERE movieid = @id
            """, new { id = movie.Id }, cancellationToken: cToken));

        foreach (var genre in movie.Genres)
        {
            await connection.ExecuteAsync(new CommandDefinition("""
                INSERT INTO genres (movieId, name)
                VALUES (@MovieId, @Name);
                """, new { MovieId = movie.Id, Name = genre }, cancellationToken: cToken));
        }

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            UPDATE movies
            SET slug = @Slug, title = @Title, yearofrelease = @YearOfRelease
            WHERE id = @Id;
            """, movie, cancellationToken: cToken));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        using var transaction = connection.BeginTransaction();

        await connection.ExecuteAsync(new CommandDefinition("""
            DELETE FROM genres WHERE movieid = @id
            """, new { id }, cancellationToken: cToken));

        var result = await connection.ExecuteAsync(new CommandDefinition("""
            DELETE FROM movies WHERE id = @id
            """, new { id }, cancellationToken: cToken));

        transaction.Commit();
        return result > 0;
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cToken)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition("SELECT count(1) FROM movies WHERE id = @id", 
            new { id }, cancellationToken: cToken));
    }
}