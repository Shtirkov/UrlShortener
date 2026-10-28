using UrlShortener.Domain;

namespace UrlShortener.Application;

public interface IShortUrlRepository
{
    Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken);
    Task<string?> GetLongUrlAsync(string code, CancellationToken cancellationToken);
}
