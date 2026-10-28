namespace UrlShortener.Domain;

public class ShortUrl
{
    public long Id { get; private set; }
    public required string Code { get; init; }
    public required string LongUrl { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
