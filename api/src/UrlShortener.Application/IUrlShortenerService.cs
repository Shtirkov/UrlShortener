using UrlShortener.Domain;

namespace UrlShortener.Application;

public interface IUrlShortenerService
{
    /// <returns>The stored short URL, or null when <paramref name="longUrl"/> is not an absolute http(s) URL.</returns>
    Task<ShortUrl?> ShortenAsync(string longUrl, CancellationToken cancellationToken = default);
    Task<string?> ResolveAsync(string code, CancellationToken cancellationToken = default);
}
