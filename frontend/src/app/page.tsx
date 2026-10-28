import { ShortenForm } from "./shorten-form";

export default function Home() {
  return (
    <main className="flex flex-1 items-center justify-center px-4 py-16">
      <div className="w-full max-w-xl">
        <h1 className="text-3xl font-semibold tracking-tight text-zinc-50">Shorten a link</h1>
        <p className="mt-2 text-zinc-400">Paste a long URL and get a short one you can share.</p>
        <ShortenForm />
      </div>
    </main>
  );
}
