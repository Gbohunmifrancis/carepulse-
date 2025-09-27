"use client";

import { useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { HeartIcon, EyeIcon, EyeSlashIcon } from "@heroicons/react/24/outline";

export default function LoginPage() {
  const router = useRouter();
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });
  const [error, setError] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);
    setError("");

    try {
      // This will connect to your API
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData),
      });

      if (response.ok) {
        const data = await response.json();
        // Store token in localStorage or cookies
        localStorage.setItem('token', data.token);
        // Redirect to dashboard
        router.push('/dashboard');
      } else {
        const errorData = await response.json();
        setError(errorData.message || 'Login failed');
      }
    } catch (error) {
      setError('Network error. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-blue-50 flex flex-col justify-center py-12 sm:px-6 lg:px-8">
      {/* Background decorative elements */}
      <div className="absolute inset-0 overflow-hidden">
        <div className="absolute -top-4 -right-4 w-72 h-72 bg-sky-300 rounded-full mix-blend-multiply filter blur-xl opacity-10 animate-blob"></div>
        <div className="absolute -bottom-8 -left-4 w-72 h-72 bg-blue-300 rounded-full mix-blend-multiply filter blur-xl opacity-10 animate-blob" style={{ animationDelay: '2s' }}></div>
        <div className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-72 h-72 bg-cyan-300 rounded-full mix-blend-multiply filter blur-xl opacity-10 animate-blob" style={{ animationDelay: '4s' }}></div>
      </div>

      <div className="relative sm:mx-auto sm:w-full sm:max-w-md">
        {/* Logo and header */}
        <div className="text-center">
          <Link href="/" className="inline-flex items-center space-x-2 mb-6">
            <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-xl flex items-center justify-center">
              <HeartIcon className="w-7 h-7 text-white" />
            </div>
            <span className="text-3xl font-bold text-gray-900">
              Care<span className="text-sky-600">Pulse</span>
            </span>
          </Link>
          <h2 className="text-3xl font-bold text-gray-900 mb-2">
            Welcome back
          </h2>
          <p className="text-gray-600">
            Sign in to your CarePulse account
          </p>
        </div>

        {/* Login form */}
        <div className="mt-8">
          <div className="bg-white/80 backdrop-blur-sm py-8 px-4 shadow-xl rounded-2xl border border-sky-100 sm:px-10">
            <form className="space-y-6" onSubmit={handleSubmit}>
              {error && (
                <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-lg text-sm">
                  {error}
                </div>
              )}

              <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                  Email address
                </label>
                <div className="mt-1">
                  <input
                    id="email"
                    name="email"
                    type="email"
                    autoComplete="email"
                    required
                    value={formData.email}
                    onChange={handleInputChange}
                    className="appearance-none block w-full px-3 py-3 border border-gray-300 rounded-lg placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent transition-all duration-200"
                    placeholder="Enter your email"
                  />
                </div>
              </div>

              <div>
                <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                  Password
                </label>
                <div className="mt-1 relative">
                  <input
                    id="password"
                    name="password"
                    type={showPassword ? "text" : "password"}
                    autoComplete="current-password"
                    required
                    value={formData.password}
                    onChange={handleInputChange}
                    className="appearance-none block w-full px-3 py-3 pr-10 border border-gray-300 rounded-lg placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent transition-all duration-200"
                    placeholder="Enter your password"
                  />
                  <button
                    type="button"
                    className="absolute inset-y-0 right-0 pr-3 flex items-center"
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? (
                      <EyeSlashIcon className="h-5 w-5 text-gray-400" />
                    ) : (
                      <EyeIcon className="h-5 w-5 text-gray-400" />
                    )}
                  </button>
                </div>
              </div>

              <div className="flex items-center justify-between">
                <div className="flex items-center">
                  <input
                    id="remember-me"
                    name="remember-me"
                    type="checkbox"
                    className="h-4 w-4 text-sky-600 focus:ring-sky-500 border-gray-300 rounded"
                  />
                  <label htmlFor="remember-me" className="ml-2 block text-sm text-gray-900">
                    Remember me
                  </label>
                </div>

                <div className="text-sm">
                  <a href="#" className="font-medium text-sky-600 hover:text-sky-500">
                    Forgot your password?
                  </a>
                </div>
              </div>

              <div>
                <button
                  type="submit"
                  disabled={isLoading}
                  className="group relative w-full flex justify-center py-3 px-4 border border-transparent text-sm font-medium rounded-lg text-white bg-gradient-to-r from-sky-500 to-blue-600 hover:from-sky-600 hover:to-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-sky-500 disabled:opacity-50 disabled:cursor-not-allowed transition-all duration-200 shadow-lg hover:shadow-xl"
                >
                  {isLoading ? (
                    <div className="flex items-center">
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                      Signing in...
                    </div>
                  ) : (
                    'Sign in'
                  )}
                </button>
              </div>
            </form>

            <div className="mt-6">
              <div className="relative">
                <div className="absolute inset-0 flex items-center">
                  <div className="w-full border-t border-gray-300" />
                </div>
                <div className="relative flex justify-center text-sm">
                  <span className="px-2 bg-white text-gray-500">Demo Credentials</span>
                </div>
              </div>

              <div className="mt-6 grid grid-cols-1 gap-3">
                <div className="bg-sky-50 border border-sky-200 rounded-lg p-4 text-sm">
                  <div className="font-medium text-sky-900 mb-2">Demo Accounts:</div>
                  <div className="text-sky-700 space-y-1">
                    <div>📊 Admin: admin@carepulse.com / admin123</div>
                    <div>👨‍⚕️ Doctor: doctor@carepulse.com / doctor123</div>
                    <div>💊 Pharmacist: pharmacist@carepulse.com / pharma123</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Footer */}
      <div className="relative mt-8 text-center">
        <p className="text-sm text-gray-500">
          Don&apos;t have an account?{' '}
          <Link href="/register" className="font-medium text-sky-600 hover:text-sky-500">
            Contact your administrator
          </Link>
        </p>
      </div>
    </div>
  );
}