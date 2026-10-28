import { cleanup, render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, describe, expect, it, vi } from "vitest";
import { shortenUrl } from "./actions";
import { ShortenForm } from "./shorten-form";

vi.mock("./actions", () => ({ shortenUrl: vi.fn() }));

const shortenUrlMock = vi.mocked(shortenUrl);

async function submit(url: string) {
  const user = userEvent.setup();
  render(<ShortenForm />);
  await user.type(screen.getByLabelText("Long URL"), url);
  await user.click(screen.getByRole("button", { name: "Shorten" }));
  return user;
}

describe("ShortenForm", () => {
  afterEach(() => {
    cleanup();
    shortenUrlMock.mockReset();
  });

  it("submits the url and shows the short url", async () => {
    shortenUrlMock.mockResolvedValue({
      status: "success",
      shortUrl: "http://s.test/abc1234",
      longUrl: "https://example.com/long",
    });

    await submit("https://example.com/long");

    const link = await screen.findByRole("link", { name: "http://s.test/abc1234" });
    expect(link.getAttribute("href")).toBe("http://s.test/abc1234");
    expect(screen.getByText("https://example.com/long")).toBeTruthy();
    expect(shortenUrlMock.mock.calls[0][1].get("url")).toBe("https://example.com/long");
  });

  it("shows the error message returned by the action", async () => {
    shortenUrlMock.mockResolvedValue({ status: "error", message: "Enter a valid http or https URL." });

    await submit("https://example.com");

    expect((await screen.findByRole("alert")).textContent).toBe("Enter a valid http or https URL.");
    expect(screen.queryByRole("link")).toBeNull();
  });

  it("keeps the entered url after submitting", async () => {
    shortenUrlMock.mockResolvedValue({ status: "error", message: "Something went wrong. Please try again." });

    await submit("https://example.com/keep");

    await screen.findByRole("alert");
    expect((screen.getByLabelText("Long URL") as HTMLInputElement).value).toBe("https://example.com/keep");
  });

  it("copies the short url to the clipboard", async () => {
    shortenUrlMock.mockResolvedValue({
      status: "success",
      shortUrl: "http://s.test/abc1234",
      longUrl: "https://example.com",
    });

    const user = await submit("https://example.com");
    await user.click(await screen.findByRole("button", { name: "Copy" }));

    expect(await navigator.clipboard.readText()).toBe("http://s.test/abc1234");
    expect(screen.getByRole("button", { name: "Copied" })).toBeTruthy();
  });
});
