using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Api.Controllers;
using UrlShortener.Infrastructure;

namespace UrlShortener.IntegrationTests;

public class UrlsApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Post_ValidUrl_Returns201WithShortUrl()
    {
        var response = await _client.PostAsJsonAsync("/api/urls", new { url = "https://example.com/a" }, Ct);
        var body = await response.Content.ReadFromJsonAsync<ShortenResponse>(Ct);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(7, body.Code.Length);
        Assert.Equal($"{ApiFactory.BaseUrl}/{body.Code}", body.ShortUrl);
        Assert.Equal("https://example.com/a", body.LongUrl);
        Assert.Equal(body.ShortUrl, response.Headers.Location?.ToString());
    }

    [Theory]
    [InlineData("not a url")]
    [InlineData("ftp://example.com")]
    [InlineData("")]
    public async Task Post_InvalidUrl_Returns400(string url)
    {
        var response = await _client.PostAsJsonAsync("/api/urls", new { url }, Ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_MissingUrl_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/urls", new { }, Ct);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_KnownCode_RedirectsToLongUrl()
    {
        var created = await ShortenAsync("https://example.com/redirect?x=1");

        var response = await _client.GetAsync($"/{created.Code}", Ct);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("https://example.com/redirect?x=1", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Get_UnknownCode_Returns404()
    {
        var response = await _client.GetAsync("/nope123", Ct);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_CachesUrlInRedis_AndServesFromCacheWithoutDatabase()
    {
        var created = await ShortenAsync("https://example.com/cached");
        await _client.GetAsync($"/{created.Code}", Ct);

        using var scope = factory.Services.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<IDistributedCache>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Equal("https://example.com/cached", await cache.GetStringAsync(created.Code, Ct));

        await db.ShortUrls.Where(x => x.Code == created.Code).ExecuteDeleteAsync(Ct);
        var response = await _client.GetAsync($"/{created.Code}", Ct);

        Assert.Equal(HttpStatusCode.Found, response.StatusCode);
        Assert.Equal("https://example.com/cached", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Database_HasUniqueIndexOnCode()
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var indexes = await db.Database
            .SqlQuery<string>($"SELECT indexdef AS \"Value\" FROM pg_indexes WHERE tablename = 'ShortUrls'")
            .ToListAsync(Ct);

        Assert.Contains(indexes, x => x.Contains("UNIQUE INDEX") && x.Contains("(\"Code\")"));
    }

    private async Task<ShortenResponse> ShortenAsync(string url)
    {
        var response = await _client.PostAsJsonAsync("/api/urls", new { url }, Ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ShortenResponse>(Ct))!;
    }
}
