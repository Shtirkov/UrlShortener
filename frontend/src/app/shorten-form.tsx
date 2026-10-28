"use client";

import { useActionState, useState } from "react";
import { shortenUrl, type ShortenState } from "./actions";

const initialState: ShortenState = { status: "idle" };

export function ShortenForm() {
  const [state, formAction, pending] = useActionState(shortenUrl, initialState);
  const [url, setUrl] = useState("");
  const [copied, setCopied] = useState(false);

  async function copy(shortUrl: string) {
    await navigator.clipboard.writeText(shortUrl);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  }

  return (
    <>
      <form action={formAction} className="mt-8 flex flex-col gap-3 sm:flex-row">
        <input
          type="url"
          name="url"
          required
          aria-label="Long URL"
          placeholder="https://example.com/a/very/long/link"
          value={url}
          onChange={(e) => setUrl(e.target.value)}
          className="min-w-0 flex-1 rounded-lg border border-zinc-800 bg-zinc-900 px-4 py-3 text-zinc-100 outline-none transition placeholder:text-zinc-600 focus:border-emerald-500/60 focus:ring-2 focus:ring-emerald-500/20"
        />
        <button
          type="submit"
          disabled={pending}
          className="rounded-lg bg-emerald-500 px-6 py-3 font-medium text-zinc-950 transition hover:bg-emerald-400 disabled:cursor-wait disabled:opacity-60"
        >
          {pending ? "Shortening..." : "Shorten"}
        </button>
      </form>

      {state.status === "error" && (
        <p role="alert" className="mt-3 text-sm text-red-400">
          {state.message}
        </p>
      )}

      {state.status === "success" && (
        <div className="mt-6 rounded-lg border border-zinc-800 bg-zinc-900/60 p-4">
          <div className="flex items-center justify-between gap-3">
            <a
              href={state.shortUrl}
              target="_blank"
              rel="noreferrer"
              className="truncate font-mono text-emerald-400 hover:underline"
            >
              {state.shortUrl}
            </a>
            <button
              type="button"
              onClick={() => copy(state.shortUrl)}
              className="shrink-0 rounded-md border border-zinc-700 px-3 py-1.5 text-sm text-zinc-300 transition hover:bg-zinc-800"
            >
              {copied ? "Copied" : "Copy"}
            </button>
          </div>
          <p className="mt-2 truncate text-sm text-zinc-500">{state.longUrl}</p>
        </div>
      )}
    </>
  );
}
