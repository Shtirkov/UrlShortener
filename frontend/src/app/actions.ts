"use server";

export type ShortenState =
  | { status: "idle" }
  | { status: "success"; shortUrl: string; longUrl: string }
  | { status: "error"; message: string };

export async function shortenUrl(_prev: ShortenState, formData: FormData): Promise<ShortenState> {
  const apiUrl = process.env.API_URL ?? "http://localhost:8080";
  const response = await fetch(`${apiUrl}/api/urls`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ url: String(formData.get("url") ?? "").trim() }),
  }).catch(() => null);

  if (response?.status === 400) return { status: "error", message: "Enter a valid http or https URL." };
  if (!response?.ok) return { status: "error", message: "Something went wrong. Please try again." };

  const { shortUrl, longUrl } = await response.json();
  return { status: "success", shortUrl, longUrl };
}
