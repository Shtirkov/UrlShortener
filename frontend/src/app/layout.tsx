import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { Background } from "./background";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "URL Shortener",
  description: "Turn long URLs into short ones.",
};

export default function RootLayout({ children }: LayoutProps<"/">) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="isolate min-h-full flex flex-col bg-zinc-950 font-sans text-zinc-100">
        <Background />
        {children}
      </body>
    </html>
  );
}
