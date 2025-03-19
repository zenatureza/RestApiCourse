namespace Movies.Application.Models;

public class GetAllMoviesOption
{
    public string? Title { get; init; }
    public int? YearOfRelease { get; init; }
    public Guid? UserId { get; set; }
    public string? SortField { get; set; }
    public SortOrder? SortOrder { get; set; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}

public enum SortOrder
{
    Unsorted,
    Ascending,
    Descending
}