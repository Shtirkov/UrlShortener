using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application;

namespace UrlShortener.Api.Controllers;

public record ShortenRequest(string Url);

public record ShortenResponse(string Code, string ShortUrl, string LongUrl);

[ApiController]
public class UrlsController(IUrlShortenerService service, IConfiguration configuration) : ControllerBase
{
    [HttpPost("api/urls")]
    public async Task<ActionResult<ShortenResponse>> Shorten(ShortenRequest request, CancellationToken cancellationToken)
    {
        var shortUrl = await service.ShortenAsync(request.Url, cancellationToken);
        if (shortUrl is null)
            return Problem("Enter a valid http or https URL.", statusCode: StatusCodes.Status400BadRequest);

        var response = new ShortenResponse(shortUrl.Code, $"{configuration["BaseUrl"]}/{shortUrl.Code}", shortUrl.LongUrl);
        return Created(response.ShortUrl, response);
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Visit(string code, CancellationToken cancellationToken) =>
        await service.ResolveAsync(code, cancellationToken) is { } longUrl ? Redirect(longUrl) : NotFound();
}
