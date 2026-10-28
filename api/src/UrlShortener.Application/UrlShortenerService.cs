using Microsoft.Extensions.Caching.Distributed;
using UrlShortener.Domain;

namespace UrlShortener.Application;

public class UrlShortenerService(IShortUrlRepository repository, IDistributedCache cache, ICodeGenerator codeGenerator) : IUrlShortenerService
{
    private static readonly DistributedCacheEntryOptions CacheOptions = new() { SlidingExpiration = TimeSpan.FromHours(1) };

    public async Task<ShortUrl?> ShortenAsync(string longUrl, CancellationToken cancellationToken = default)
    {
        if (!Uri.TryCreate(longUrl, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            return null;

        var shortUrl = new ShortUrl { Code = codeGenerator.Generate(), LongUrl = longUrl };
        await repository.AddAsync(shortUrl, cancellationToken);
        return shortUrl;
    }

    public async Task<string?> ResolveAsync(string code, CancellationToken cancellationToken = default)
    {
        var cached = await cache.GetStringAsync(code, cancellationToken);
        if (cached is not null)
            return cached;

        var longUrl = await repository.GetLongUrlAsync(code, cancellationToken);
        if (longUrl is not null)
            await cache.SetStringAsync(code, longUrl, CacheOptions, cancellationToken);

        return longUrl;
    }
}
