using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

//[ApiController]
//[ApiVersion(1.0)]
//public class MoviesController : ControllerBase
//{
//    private readonly IMovieService _movieService;
//    private readonly IOutputCacheStore _outputCacheStore;

//    public MoviesController(IMovieService movieService, IOutputCacheStore outputCacheStore)
//    {
//        _movieService = movieService;
//        _outputCacheStore = outputCacheStore;
//    }

//    //[Authorize(AuthConstants.TrustedMemberPolicyName)]
//    [ServiceFilter(typeof(ApiKeyAuthFilter))]
//    [HttpPost(ApiEndpoints.Movies.Create)]
//    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
//    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> Create(
//        [FromBody] CreateMovieRequest request,
//        CancellationToken cToken)
//    {
//        var movie = request.MapToMovie();
//        await _movieService.CreateAsync(movie, cToken);
//        await _outputCacheStore.EvictByTagAsync("movies", cToken);

//        return CreatedAtAction(
//            nameof(Get), new { idOrSlug = movie.Id },
//            movie.MapToResponse());
//    }

//    [HttpGet(ApiEndpoints.Movies.Get)]
//    //[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
//    [OutputCache(PolicyName = "MovieCache")]
//    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> Get(
//        [FromRoute] string idOrSlug,
//        [FromServices] LinkGenerator linkGenerator,
//        CancellationToken cToken)
//    {
//        var userId = HttpContext.GetUserId();

//        var movie = Guid.TryParse(idOrSlug, out var id)
//            ? await _movieService.GetByIdAsync(id, userId, cToken)
//            : await _movieService.GetBySlugAsync(idOrSlug, userId, cToken);
//        if (movie is null) return NotFound();

//        var response = movie.MapToResponse();

//        var movieObj = new { id = movie.Id };
//        response.Links.Add(new Contracts.Responses.Link
//        {
//            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Get), values: new { idOrSlug = movie.Id }),
//            Rel = "self",
//            Type = "GET"
//        });

//        response.Links.Add(new Contracts.Responses.Link
//        {
//            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Update), values: movieObj),
//            Rel = "self",
//            Type = "PUT"
//        });

//        response.Links.Add(new Contracts.Responses.Link
//        {
//            Href = linkGenerator.GetPathByAction(HttpContext, nameof(Delete), values: movieObj),
//            Rel = "self",
//            Type = "DELETE"
//        });

//        return Ok(response);
//    }

//    [HttpGet(ApiEndpoints.Movies.GetAll)]
//    //[ResponseCache(Duration = 30, VaryByQueryKeys = ["title", "yearOfRelease", "sortBy", "page", "pageSize"])]
//    [OutputCache(PolicyName = "MovieCache")]
//    [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> GetAll([FromQuery] GetAllMoviesRequest request, CancellationToken cToken)
//    {
//        var userId = HttpContext.GetUserId();
//        var options = request.MapToOptions()
//            .WithUserId(userId);

//        var count = await _movieService.CountAsync(options.Title, options.YearOfRelease, cToken);
//        var movies = await _movieService.GetAllAsync(options, cToken);
//        return Ok(movies.MapToResponse(
//            request.Page.GetValueOrDefault(PagedRequest.DefaultPage),
//            request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
//            count));
//    }

//    [Authorize(AuthConstants.TrustedMemberPolicyName)]
//    [HttpPut(ApiEndpoints.Movies.Update)]
//    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
//    public async Task<IActionResult> Update(
//        [FromRoute] Guid id,
//        [FromBody] UpdateMovieRequest request,
//        CancellationToken cToken)
//    {
//        var movie = request.MapToMovie(id);
//        var userId = HttpContext.GetUserId();
//        var updatedMovie = await _movieService.UpdateAsync(movie, userId, cToken);
//        if (updatedMovie is null) return NotFound();

//        await _outputCacheStore.EvictByTagAsync("movies", cToken);
//        var response = movie.MapToResponse();
//        return Ok(response);
//    }

//    [Authorize(AuthConstants.AdminUserPolicyName)]
//    [HttpDelete(ApiEndpoints.Movies.Delete)]
//    [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status204NoContent)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> Delete(
//        [FromRoute] Guid id,
//        CancellationToken cToken)
//    {
//        var deleted = await _movieService.DeleteAsync(id, cToken);
//        if (!deleted) return NotFound();

//        await _outputCacheStore.EvictByTagAsync("movies", cToken);
//        return NoContent();
//    }
//}
