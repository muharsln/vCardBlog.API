using Newtonsoft.Json.Linq;

namespace BasicBlogAPI;

public class GitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<JArray> GetRepositoriesAsync(string token)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/repos");
        request.Headers.Add("User-Agent", "GitHubRepoMinimalApi");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", token);

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JArray.Parse(content);
    }
}