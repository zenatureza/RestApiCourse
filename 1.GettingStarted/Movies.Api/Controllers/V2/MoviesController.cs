using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;

namespace Movies.Api.Controllers.V2;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [HttpGet(ApiEndpoints.V2.Movies.Get)]
    public async Task<IActionResult> Get(
        [FromRoute] string idOrSlug,
        [FromServices] LinkGenerator linkGenerator,
        CancellationToken cToken)
    {
        var userId = HttpContext.GetUserId();

        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, userId, cToken)
            : await _movieService.GetBySlugAsync(idOrSlug, userId, cToken);
        if (movie is null) return NotFound();

        var response = movie.MapToResponse();

        var movieObj = new { id = movie.Id };
        response.Links.Add(new Contracts.Responses.Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id }),
            Rel = "self",
            Type = "GET"
        });
        
        return Ok(response);
    }
}
