using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using UrlShortener.Application;
using UrlShortener.Domain;

namespace UrlShortener.UnitTests;

public class UrlShortenerServiceTests
{
    private readonly FakeRepository _repository = new();
    private readonly MemoryDistributedCache _cache = new(Options.Create(new MemoryDistributedCacheOptions()));
    private readonly UrlShortenerService _service;

    public UrlShortenerServiceTests() => _service = new UrlShortenerService(_repository, _cache);

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://example.com/some/path?q=1#frag")]
    public async Task ShortenAsync_ValidUrl_StoresAndReturnsShortUrl(string longUrl)
    {
        var result = await _service.ShortenAsync(longUrl);

        Assert.NotNull(result);
        Assert.Equal(longUrl, result.LongUrl);
        Assert.Equal(CodeGenerator.Length, result.Code.Length);
        Assert.Equal(longUrl, _repository.Urls[result.Code]);
    }

    [Theory]
    [InlineData("")]
    [InlineData("not a url")]
    [InlineData("example.com")]
    [InlineData("/relative/path")]
    [InlineData("ftp://example.com/file")]
    [InlineData("javascript:alert(1)")]
    public async Task ShortenAsync_InvalidUrl_ReturnsNullAndStoresNothing(string longUrl)
    {
        var result = await _service.ShortenAsync(longUrl);

        Assert.Null(result);
        Assert.Empty(_repository.Urls);
    }

    [Fact]
    public async Task ResolveAsync_CacheMiss_ReadsRepositoryAndPopulatesCache()
    {
        _repository.Urls["abc1234"] = "https://example.com";

        var result = await _service.ResolveAsync("abc1234");

        Assert.Equal("https://example.com", result);
        Assert.Equal(1, _repository.Lookups);
        Assert.Equal("https://example.com", await _cache.GetStringAsync("abc1234"));
    }

    [Fact]
    public async Task ResolveAsync_CacheHit_DoesNotReadRepository()
    {
        await _cache.SetStringAsync("abc1234", "https://cached.example.com");

        var result = await _service.ResolveAsync("abc1234");

        Assert.Equal("https://cached.example.com", result);
        Assert.Equal(0, _repository.Lookups);
    }

    [Fact]
    public async Task ResolveAsync_SecondCall_IsServedFromCache()
    {
        _repository.Urls["abc1234"] = "https://example.com";

        await _service.ResolveAsync("abc1234");
        var result = await _service.ResolveAsync("abc1234");

        Assert.Equal("https://example.com", result);
        Assert.Equal(1, _repository.Lookups);
    }

    [Fact]
    public async Task ResolveAsync_UnknownCode_ReturnsNullAndCachesNothing()
    {
        var result = await _service.ResolveAsync("missing");

        Assert.Null(result);
        Assert.Null(await _cache.GetStringAsync("missing"));
    }

    private class FakeRepository : IShortUrlRepository
    {
        public Dictionary<string, string> Urls { get; } = [];
        public int Lookups { get; private set; }

        public Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken)
        {
            Urls[shortUrl.Code] = shortUrl.LongUrl;
            return Task.CompletedTask;
        }

        public Task<string?> GetLongUrlAsync(string code, CancellationToken cancellationToken)
        {
            Lookups++;
            return Task.FromResult(Urls.GetValueOrDefault(code));
        }
    }
}
