namespace Movies.Contracts.Requests.V1;

public class GetAllMoviesRequest : PagedRequest
{
    public string? Title { get; init; }
    public int? YearOfRelease { get; init; }
    public string? SortBy { get; init; }
}
