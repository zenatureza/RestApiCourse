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

    public async Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            new CommandDefinition("""
            SELECT m.*, ROUND(AVG(r.rating), 1) AS rating, myr.rating AS userrating
            FROM movies m
            LEFT JOIN ratings r ON m.id = r.movieId
            LEFT JOIN ratings myr ON m.id = myr.movieId 
                AND myr.userId = @userId
            WHERE id = @id
            GROUP BY id, userrating;
            """, new { id, userId }, cancellationToken: cToken));
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

    public async Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var movie = await connection.QueryFirstOrDefaultAsync<Movie>(
            new CommandDefinition("""
            SELECT m.*, ROUND(AVG(r.rating), 1) AS rating, myr.rating AS userrating
            FROM movies m
            LEFT JOIN ratings r ON m.id = r.movieId
            LEFT JOIN ratings myr ON m.id = myr.movieId 
                AND myr.userId = @userId
            WHERE slug = @slug
            GROUP BY id, userrating;
            """, new { slug, userId }, cancellationToken: cToken));
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

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOption options, CancellationToken cToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cToken);
        var orderClause = string.Empty;
        if (options.SortField is not null)
        {
            orderClause = $"""
                , m.{options.SortField}
                order by m.{options.SortField} {(options.SortOrder == SortOrder.Descending ? "DESC" : "ASC")}
                """;
        }

        var result = await connection.QueryAsync(new CommandDefinition($"""
            SELECT 
                m.*, 
                STRING_AGG(DISTINCT g.name, ',') AS genres,
                ROUND(AVG(r.rating), 1) AS rating, 
                myr.rating AS userrating
            FROM movies m 
            LEFT JOIN genres g ON m.id = g.movieId
            LEFT JOIN ratings r ON m.id = r.movieId
            LEFT JOIN ratings myr ON m.id = myr.movieId 
                AND myr.userId = @userId
            WHERE (@title IS NULL OR m.title LIKE ('%' || @title || '%'))
                AND (@yearofrelease IS NULL OR m.yearofrelease = @yearofrelease)
            GROUP BY m.id, myr.rating {orderClause}
            """, new 
            { 
                userId = options.UserId,
                title = options.Title,
                yearofrelease = options.YearOfRelease
            } ,cancellationToken: cToken));

        return result.Select(x => new Movie 
        {
            Id = x.id,
            Title = x.title,
            YearOfRelease = x.yearofrelease,
            Rating = (float?)x.rating,
            UserRating = (int?)x.userrating,
            Genres = Enumerable.ToList(x.genres.Split(','))
        });
    }

    public async Task<bool> UpdateAsync(Movie movie, CancellationToken cToken = default)
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