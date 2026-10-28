import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { shortenUrl } from "./actions";

const fetchMock = vi.fn();

function submit(url: string) {
  const formData = new FormData();
  formData.set("url", url);
  return shortenUrl({ status: "idle" }, formData);
}

describe("shortenUrl", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock);
    vi.stubEnv("API_URL", "http://api.test");
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.unstubAllEnvs();
    fetchMock.mockReset();
  });

  it("posts the trimmed url to the api and returns the short url", async () => {
    fetchMock.mockResolvedValue(
      Response.json({ code: "abc1234", shortUrl: "http://s.test/abc1234", longUrl: "https://example.com" }, { status: 201 }),
    );

    const result = await submit("  https://example.com  ");

    expect(fetchMock).toHaveBeenCalledWith("http://api.test/api/urls", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ url: "https://example.com" }),
    });
    expect(result).toEqual({ status: "success", shortUrl: "http://s.test/abc1234", longUrl: "https://example.com" });
  });

  it("returns a validation message when the api rejects the url", async () => {
    fetchMock.mockResolvedValue(new Response(null, { status: 400 }));

    expect(await submit("nope")).toEqual({ status: "error", message: "Enter a valid http or https URL." });
  });

  it("returns a generic error when the api fails", async () => {
    fetchMock.mockResolvedValue(new Response(null, { status: 500 }));

    expect(await submit("https://example.com")).toEqual({
      status: "error",
      message: "Something went wrong. Please try again.",
    });
  });

  it("returns a generic error when the api is unreachable", async () => {
    fetchMock.mockRejectedValue(new TypeError("fetch failed"));

    expect(await submit("https://example.com")).toEqual({
      status: "error",
      message: "Something went wrong. Please try again.",
    });
  });
});
