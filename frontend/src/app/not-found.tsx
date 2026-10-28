import Link from "next/link";

export default function NotFound() {
  return (
    <main className="flex flex-1 items-center justify-center px-4 py-16">
      <div className="w-full max-w-xl">
        <p className="font-mono text-sm text-emerald-400">404</p>
        <h1 className="mt-2 text-3xl font-semibold tracking-tight text-zinc-50">Link not found</h1>
        <p className="mt-2 text-zinc-400">This short link does not exist. Check that it was copied correctly.</p>
        <Link
          href="/"
          className="mt-8 inline-block rounded-lg bg-emerald-500 px-6 py-3 font-medium text-zinc-950 transition hover:bg-emerald-400"
        >
          Shorten a link
        </Link>
      </div>
    </main>
  );
}
