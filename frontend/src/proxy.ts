import { NextResponse, type NextRequest } from "next/server";

export async function proxy(request: NextRequest) {
  const apiUrl = process.env.API_URL ?? "http://localhost:8080";
  const response = await fetch(`${apiUrl}${request.nextUrl.pathname}`, { redirect: "manual" }).catch(() => null);
  const location = response?.headers.get("location");

  // Unknown codes fall through to the app, which has no matching route and renders not-found.tsx.
  return location ? NextResponse.redirect(location, response!.status) : NextResponse.next();
}

export const config = {
  matcher: "/:code([0-9A-Za-z]+)",
};
