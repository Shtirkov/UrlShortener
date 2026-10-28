export function Background() {
  return (
    <div aria-hidden className="pointer-events-none fixed inset-0 -z-10 overflow-hidden">
      <div className="absolute inset-0 bg-[linear-gradient(to_right,rgb(255_255_255/0.03)_1px,transparent_1px),linear-gradient(to_bottom,rgb(255_255_255/0.03)_1px,transparent_1px)] bg-size-[48px_48px] mask-[radial-gradient(ellipse_at_center,black,transparent_70%)]" />
      <div className="absolute -top-48 -left-40 size-144 rounded-full bg-emerald-500/15 blur-3xl motion-safe:animate-drift" />
      <div className="absolute -right-40 -bottom-56 size-128 rounded-full bg-teal-400/10 blur-3xl [animation-delay:-12s] motion-safe:animate-drift" />
    </div>
  );
}
