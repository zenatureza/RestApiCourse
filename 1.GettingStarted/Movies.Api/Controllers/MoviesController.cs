﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

//[Authorize]
[ApiController]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;

    public MoviesController(IMovieService movieService)
    {
        _movieService = movieService;
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPost(ApiEndpoints.Movies.Create)]
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

    [HttpGet(ApiEndpoints.Movies.Get)]
    public async Task<IActionResult> Get(
        [FromRoute] string idOrSlug,
        CancellationToken cToken)
    {
        var movie = Guid.TryParse(idOrSlug, out var id)
            ? await _movieService.GetByIdAsync(id, cToken)
            : await _movieService.GetBySlugAsync(idOrSlug, cToken);
        if (movie is null) return NotFound();

        return Ok(movie.MapToResponse());
    }

    [HttpGet(ApiEndpoints.Movies.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cToken)
    {
        var movies = await _movieService.GetAllAsync(cToken);
        return Ok(movies.MapToResponse());
    }

    [Authorize(AuthConstants.TrustedMemberPolicyName)]
    [HttpPut(ApiEndpoints.Movies.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateMovieRequest request,
        CancellationToken cToken)
    {
        var movie = request.MapToMovie(id);
        var updatedMovie = await _movieService.UpdateAsync(movie, cToken);
        if (updatedMovie is null) return NotFound();

        var response = movie.MapToResponse();
        return Ok(response);
    }

    [Authorize(AuthConstants.AdminUserPolicyName)]
    [HttpDelete(ApiEndpoints.Movies.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cToken)
    {
        var deleted = await _movieService.DeleteAsync(id, cToken);
        if (!deleted) return NotFound();

        return NoContent();
    }
}
