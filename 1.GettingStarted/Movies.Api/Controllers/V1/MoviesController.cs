using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;

namespace Movies.Api.Controllers.V1;

[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.V1.Movies.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateMovieRequest request,
        CancellationToken cToken)
    {
        var movie = request.MapToMovie();
        await _movieService.CreateAsync(movie, cToken);

        return CreatedAtAction(
            nameof(Get), new { idOrSlug = movie.Id }, 
            movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.V1.Movies.Get)]
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

        response.Links.Add(new Contracts.Responses.Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj),
            Rel = "self",
            Type = "PUT"
        });

        response.Links.Add(new Contracts.Responses.Link
        {
            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: movieObj),
            Rel = "self",
            Type = "DELETE"
        });

        return Ok(response);
    }

    [HttpGet(ApiEndpoints.V1.Movies.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken cToken)
    {
        var userId = HttpContext.GetUserId();
        var options = request.MapToOptions()
            .WithUserId(userId);

        var count = await _movieService.CountAsync(options.Title, options.YearOfRelease, cToken);
        var movies = await _movieService.GetAllAsync(options, cToken);
        return Ok(movies.MapToResponse(request.Page, request.PageSize, count));
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.V1.Movies.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken cToken)
    {
        var movie = request.MapToMovie(id);
        var userId = HttpContext.GetUserId();
        var updatedMovie = await _movieService.UpdateAsync(movie, userId, cToken);
        if (updatedMovie is null) return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.V1.Movies.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cToken)
    {
        var deleted = await _movieService.DeleteAsync(id, cToken);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
