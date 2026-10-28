// @vitest-environment node
import { NextRequest } from "next/server";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { proxy } from "./proxy";

const fetchMock = vi.fn();

function visit(path: string) {
  return proxy(new NextRequest(`http://s.test${path}`));
}

describe("proxy", () => {
  beforeEach(() => {
    vi.stubGlobal("fetch", fetchMock);
    vi.stubEnv("API_URL", "http://api.test");
  });

  afterEach(() => {
    vi.unstubAllGlobals();
    vi.unstubAllEnvs();
    fetchMock.mockReset();
  });

  it("redirects known codes to the long url", async () => {
    fetchMock.mockResolvedValue(new Response(null, { status: 302, headers: { location: "https://example.com/long" } }));

    const response = await visit("/abc1234");

    expect(fetchMock).toHaveBeenCalledWith("http://api.test/abc1234", { redirect: "manual" });
    expect(response.status).toBe(302);
    expect(response.headers.get("location")).toBe("https://example.com/long");
  });

  it("lets unknown codes through to the not-found page", async () => {
    fetchMock.mockResolvedValue(new Response(null, { status: 404 }));

    const response = await visit("/missing");

    expect(response.headers.get("x-middleware-next")).toBe("1");
  });

  it("lets requests through when the api is unreachable", async () => {
    fetchMock.mockRejectedValue(new TypeError("fetch failed"));

    const response = await visit("/abc1234");

    expect(response.headers.get("x-middleware-next")).toBe("1");
  });
});
