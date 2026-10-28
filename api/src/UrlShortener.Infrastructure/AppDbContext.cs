using Microsoft.EntityFrameworkCore;
using UrlShortener.Domain;

namespace UrlShortener.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ShortUrl> ShortUrls => Set<ShortUrl>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var shortUrl = modelBuilder.Entity<ShortUrl>();
        shortUrl.Property(x => x.Code).HasMaxLength(16);
        shortUrl.HasIndex(x => x.Code).IsUnique();
    }
}
