namespace Movies.Application.Models;

public class GetAllMoviesOption
{
    public string? Title { get; init; }
    public int? YearOfRelease { get; init; }
    public Guid? UserId { get; set; }
}
