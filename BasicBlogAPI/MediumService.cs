using System.Xml.Linq;

namespace BasicBlogAPI;

public class MediumService
{
    private readonly HttpClient _httpClient;
    private const string rssUrl = "https://medium.com/feed/@muharsln";

    public MediumService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Post>> GetPostsAsync()
    {

        var response = await _httpClient.GetStringAsync(rssUrl);
        var xml = XDocument.Parse(response);

        var posts = xml.Descendants("item").Select(item =>
        {
            var content = item.Element("{http://purl.org/rss/1.0/modules/content/}encoded")?.Value;
            var imageUrl = ExtractImageUrlFromContent(content);

            return new Post
            {
                Title = item.Element("title")?.Value,
                Link = item.Element("guid")?.Value,
                Image = imageUrl
            };
        });

        return posts;
    }

    private string ExtractImageUrlFromContent(string content)
    {
        if (string.IsNullOrEmpty(content))
            return null;

        // Basit string işleme ile img src değerini alma
        var imgTagStart = content.IndexOf("<img");
        if (imgTagStart == -1)
            return null;

        var srcStart = content.IndexOf("src=\"", imgTagStart) + 5;
        if (srcStart == 4)
            return null;

        var srcEnd = content.IndexOf("\"", srcStart);
        if (srcEnd == -1)
            return null;

        return content.Substring(srcStart, srcEnd - srcStart);
    }
}

public class Post
{
    public string? Title { get; set; }
    public string? Link { get; set; }
    public string? Image { get; set; }
}