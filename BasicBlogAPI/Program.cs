using BasicBlogAPI;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<MediumService>();
builder.Services.AddHttpClient<GitHubService>();

builder.Services.AddTransient<MediumService>();
builder.Services.AddTransient<GitHubService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            //policy.WithOrigins("http://localhost:4200")
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

app.Map("/", () => "Running");

app.MapGet("/api/repositories", async (GitHubService gitHubService) =>
{
    JArray repositories = await gitHubService.GetRepositoriesAsync(app.Configuration["Github:Token"]);

    var result = repositories.Select(repo => new
    {
        Name = repo["name"].ToString(),
        Url = repo["html_url"].ToString()
    });

    return Results.Ok(result);
});


app.MapGet("/api/posts", async (MediumService mediumService) =>
{
    try
    {
        var posts = await mediumService.GetPostsAsync();
        return Results.Ok(posts);
    }
    catch (Exception ex)
    {
        return Results.Problem("Failed to retrieve or parse RSS feed. " + ex.Message);
    }

});



app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.Run();