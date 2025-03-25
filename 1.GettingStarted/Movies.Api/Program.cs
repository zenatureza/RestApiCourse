using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Movies.Api.Auth;
using Movies.Api.Endpoints;
using Movies.Api.Health;
using Movies.Api.Mapping;
using Movies.Api.Swagger;
using Movies.Application;
using Movies.Application.Database;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
        ClockSkew = System.TimeSpan.Zero
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(AuthConstants.AdminUserPolicyName, p =>
        //p.RequireClaim(AuthConstants.AdminUserClaimName, "true"))
        p.AddRequirements(new AdminAuthRequirement(config["ApiKey"])))
    .AddPolicy(AuthConstants.TrustedMemberPolicyName, p =>
        p.RequireAssertion(c => c.User.HasClaim(m => 
            m is { Type: AuthConstants.AdminUserClaimName, Value: "true" } ||
            m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));

builder.Services.AddScoped<ApiKeyAuthFilter>();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
})
//.AddMvc()
.AddApiExplorer();

builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddResponseCaching();
builder.Services.AddOutputCache(x =>
{
    x.AddBasePolicy(c => c.Cache());
    x.AddPolicy("MovieCache", c => c.Cache()
        .Expire(TimeSpan.FromMinutes(1))
        .SetVaryByQuery(["title", "yearOfRelease", "sortBy", "page", "pageSize"])
        .Tag("movies"));
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

//builder.Services.AddControllers();
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>(DatabaseHealthCheck.Name);

builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddDatabase(config["Database:ConnectionString"]!);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName);
        }
    });
}


app.MapHealthChecks("_health");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.UseCors();
//app.UseResponseCaching();
app.UseOutputCache();

app.UseMiddleware<ValidationMappingMiddleware>();
//app.MapControllers();
app.MapApiEndpoints();

var dbInitializer = app.Services.GetRequiredService<DbInitializer>();
await dbInitializer.InitializeAsync();

app.Run();