﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Auth;
using Movies.Api.Mapping;
using Movies.Application.Services;
using Movies.Contracts.Requests.V1;
using Movies.Contracts.Responses;

namespace Movies.Api.Controllers;

//[ApiController]
//[ApiVersion(1.0)]
//public class RatingsController : ControllerBase
//{
//    private readonly IRatingService _ratingService;

//    public RatingsController(IRatingService ratingService)
//    {
//        _ratingService = ratingService;
//    }

//    [Authorize]
//    [HttpPut(ApiEndpoints.Movies.Rate)]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> RateMovie([FromRoute] Guid id,
//        [FromBody] RateMovieRequest request,
//        CancellationToken cToken)
//    {
//        var userId = HttpContext.GetUserId();
//        var result = await _ratingService.RateMovieAsync(id, request.Rating, userId!.Value, cToken);
//        return result ? Ok() : NotFound();
//    }

//    [Authorize]
//    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
//    [ProducesResponseType(StatusCodes.Status200OK)]
//    [ProducesResponseType(StatusCodes.Status404NotFound)]
//    public async Task<IActionResult> DeleteRating([FromRoute] Guid id,
//        CancellationToken cToken)
//    {
//        var userId = HttpContext.GetUserId();
//        var result = await _ratingService.DeleteRatingAsync(id, userId!.Value, cToken);
//        return result ? Ok() : NotFound();
//    }

//    [Authorize]
//    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
//    [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>),StatusCodes.Status200OK)]
//    public async Task<IActionResult> GetUserRatings(CancellationToken cToken)
//    {
//        var userId = HttpContext.GetUserId();
//        var ratings = await _ratingService.GetRatingsForUserAsync(userId!.Value, cToken);
//        var response = ratings.MapToResponse();

//        return Ok(response);
//    }
//}