import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "CarePulse - Pharmacy Management System",
  description: "Modern pharmacy management and prescription tracking system",
  keywords: "pharmacy, prescription, healthcare, medical, inventory",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${inter.className} antialiased bg-gradient-to-br from-sky-50 to-blue-50 min-h-screen`}>
        {children}
      </body>
    </html>
  );
}
