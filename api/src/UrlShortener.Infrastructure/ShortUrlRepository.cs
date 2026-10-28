using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Domain;

namespace UrlShortener.Infrastructure;

public class ShortUrlRepository(AppDbContext db) : IShortUrlRepository
{
    public async Task AddAsync(ShortUrl shortUrl, CancellationToken cancellationToken)
    {
        db.ShortUrls.Add(shortUrl);
        await db.SaveChangesAsync(cancellationToken);
    }

    public Task<string?> GetLongUrlAsync(string code, CancellationToken cancellationToken) =>
        db.ShortUrls.Where(x => x.Code == code).Select(x => x.LongUrl).FirstOrDefaultAsync(cancellationToken);
}
