import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import Link from "next/link";
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
  title: "Clothing Store - Fashion & Style",
  description: "Discover the latest fashion trends at our clothing store",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body
        className={`${geistSans.variable} ${geistMono.variable} antialiased`}
      >
        <nav className="bg-gradient-to-r from-purple-600 via-pink-600 to-red-600 text-white shadow-lg">
           <div className="container mx-auto px-6 py-4 flex justify-between items-center">
             <h1 className="text-2xl font-bold tracking-wide">FashionHub</h1>
             <div className="space-x-6">
               <Link href="/" className="hover:text-yellow-300 transition-colors duration-300 font-medium">Home</Link>
               <Link href="/products/new" className="bg-white text-purple-600 px-4 py-2 rounded-full hover:bg-yellow-300 hover:text-purple-700 transition-all duration-300 font-medium shadow-md">Add Product</Link>
             </div>
           </div>
         </nav>
        {children}
      </body>
    </html>
  );
}
