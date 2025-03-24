using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Sdk;
using Refit;
using System.Text.Json;

//var moviesApi = RestService.For<IMoviesApi>("https://localhost:5001");
//var movie = await moviesApi.GetMovieAsync("eecb7cb0-d570-4112-9b2f-a0e67a21204a");
//Console.WriteLine(JsonSerializer.Serialize(movie));

var services = new ServiceCollection();

services.AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(x => 
        x.BaseAddress = new Uri("https://localhost:5001"));
var provider = services.BuildServiceProvider();
var moviesApi = provider.GetRequiredService<IMoviesApi>();

var movies = await moviesApi.GetMoviesAsync(new Movies.Contracts.Requests.V1.GetAllMoviesRequest 
{ 
    Page = 1,
    PageSize = 10,
    SortBy =null,
    Title =null,
    YearOfRelease =null,
});
foreach (var item in movies.Items)
{
    Console.WriteLine(JsonSerializer.Serialize(item));
}