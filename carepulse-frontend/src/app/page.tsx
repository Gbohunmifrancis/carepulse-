import Image from "next/image";
import Link from "next/link";
import { 
  HeartIcon, 
  ShieldCheckIcon, 
  ClockIcon,
  UserGroupIcon,
  ChartBarIcon,
  CpuChipIcon
} from "@heroicons/react/24/outline";

export default function HomePage() {
  return (
    <div className="min-h-screen bg-gradient-to-br from-sky-50 via-white to-blue-50">
      {/* Navigation */}
      <nav className="relative z-10 bg-white/80 backdrop-blur-md border-b border-sky-100">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-4">
            <div className="flex items-center space-x-2">
              <div className="w-10 h-10 bg-gradient-to-r from-sky-500 to-blue-600 rounded-xl flex items-center justify-center">
                <HeartIcon className="w-6 h-6 text-white" />
              </div>
              <span className="text-2xl font-bold text-gray-900">
                Care<span className="text-sky-600">Pulse</span>
              </span>
            </div>
            <div className="hidden md:flex space-x-8">
              <a href="#features" className="text-gray-600 hover:text-sky-600 transition-colors">Features</a>
              <a href="#about" className="text-gray-600 hover:text-sky-600 transition-colors">About</a>
              <a href="#contact" className="text-gray-600 hover:text-sky-600 transition-colors">Contact</a>
            </div>
            <Link 
              href="/login" 
              className="bg-gradient-to-r from-sky-500 to-blue-600 text-white px-6 py-2 rounded-lg hover:from-sky-600 hover:to-blue-700 transition-all duration-200 shadow-lg hover:shadow-xl"
            >
              Get Started
            </Link>
          </div>
        </div>
      </nav>

      {/* Hero Section */}
      <main className="relative">
        {/* Background decorative elements */}
        <div className="absolute inset-0 overflow-hidden">
          <div className="absolute -top-4 -right-4 w-72 h-72 bg-sky-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob"></div>
          <div className="absolute -bottom-8 -left-4 w-72 h-72 bg-blue-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob" style={{ animationDelay: '2s' }}></div>
          <div className="absolute top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-72 h-72 bg-cyan-300 rounded-full mix-blend-multiply filter blur-xl opacity-20 animate-blob" style={{ animationDelay: '4s' }}></div>
        </div>

        <div className="relative max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 pt-20 pb-16">
          <div className="text-center">
            <div className="mb-8 inline-flex items-center px-4 py-2 bg-sky-100 rounded-full text-sky-700 text-sm font-medium">
              🚀 Welcome to the Future of Pharmacy Management
            </div>
            
            <h1 className="text-4xl md:text-6xl lg:text-7xl font-bold text-gray-900 mb-8 leading-tight">
              Modern Healthcare
              <br />
              <span className="bg-gradient-to-r from-sky-600 to-blue-600 bg-clip-text text-transparent">
                Management
              </span>
            </h1>
            
            <p className="text-xl md:text-2xl text-gray-600 mb-12 max-w-3xl mx-auto leading-relaxed">
              Streamline your pharmacy operations with our comprehensive management system. 
              Handle prescriptions, inventory, and patient care with confidence and efficiency.
            </p>

            <div className="flex flex-col sm:flex-row gap-4 justify-center items-center mb-16">
              <Link 
                href="/login"
                className="bg-gradient-to-r from-sky-500 to-blue-600 text-white px-8 py-4 rounded-xl hover:from-sky-600 hover:to-blue-700 transition-all duration-200 shadow-lg hover:shadow-xl text-lg font-semibold transform hover:scale-105"
              >
                Get Started Now
              </Link>
              <button className="border-2 border-sky-200 text-sky-600 px-8 py-4 rounded-xl hover:bg-sky-50 transition-all duration-200 text-lg font-semibold">
                Watch Demo
              </button>
            </div>

            {/* Hero Image/Illustration Placeholder */}
            <div className="relative mx-auto max-w-5xl">
              <div className="bg-white/80 backdrop-blur-sm rounded-2xl shadow-2xl border border-sky-100 p-8">
                <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                  <div className="bg-gradient-to-br from-sky-50 to-blue-50 rounded-xl p-6 text-center">
                    <div className="w-12 h-12 bg-sky-500 rounded-lg mx-auto mb-4 flex items-center justify-center">
                      <ChartBarIcon className="w-6 h-6 text-white" />
                    </div>
                    <h3 className="font-semibold text-gray-900 mb-2">Analytics Dashboard</h3>
                    <p className="text-gray-600 text-sm">Real-time insights and reporting</p>
                  </div>
                  <div className="bg-gradient-to-br from-sky-50 to-blue-50 rounded-xl p-6 text-center">
                    <div className="w-12 h-12 bg-blue-500 rounded-lg mx-auto mb-4 flex items-center justify-center">
                      <ShieldCheckIcon className="w-6 h-6 text-white" />
                    </div>
                    <h3 className="font-semibold text-gray-900 mb-2">Secure & Compliant</h3>
                    <p className="text-gray-600 text-sm">HIPAA compliant and secure</p>
                  </div>
                  <div className="bg-gradient-to-br from-sky-50 to-blue-50 rounded-xl p-6 text-center">
                    <div className="w-12 h-12 bg-cyan-500 rounded-lg mx-auto mb-4 flex items-center justify-center">
                      <CpuChipIcon className="w-6 h-6 text-white" />
                    </div>
                    <h3 className="font-semibold text-gray-900 mb-2">Smart Automation</h3>
                    <p className="text-gray-600 text-sm">Automated inventory management</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Features Section */}
        <section id="features" className="py-20 bg-white/50">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="text-center mb-16">
              <h2 className="text-3xl md:text-4xl font-bold text-gray-900 mb-4">
                Everything You Need to Manage Your Pharmacy
              </h2>
              <p className="text-xl text-gray-600 max-w-2xl mx-auto">
                Comprehensive tools designed specifically for modern healthcare professionals
              </p>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-shadow border border-sky-100">
                <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center mb-6">
                  <HeartIcon className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-3">Prescription Management</h3>
                <p className="text-gray-600">
                  Efficiently handle prescriptions from doctors, track patient medication history, 
                  and ensure accurate dispensing with our intuitive system.
                </p>
              </div>

              <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-shadow border border-sky-100">
                <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center mb-6">
                  <ChartBarIcon className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-3">Inventory Control</h3>
                <p className="text-gray-600">
                  Real-time inventory tracking, automated reorder alerts, and expiry date 
                  monitoring to prevent stockouts and waste.
                </p>
              </div>

              <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-shadow border border-sky-100">
                <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center mb-6">
                  <UserGroupIcon className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-3">User Management</h3>
                <p className="text-gray-600">
                  Role-based access control for pharmacists, doctors, and administrators 
                  with comprehensive audit trails.
                </p>
              </div>

              <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-shadow border border-sky-100">
                <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center mb-6">
                  <ClockIcon className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-3">Real-time Analytics</h3>
                <p className="text-gray-600">
                  Comprehensive reporting and analytics to track sales, inventory turnover, 
                  and prescription patterns for better decision making.
                </p>
              </div>

              <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-shadow border border-sky-100">
                <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center mb-6">
                  <ShieldCheckIcon className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-3">Security & Compliance</h3>
                <p className="text-gray-600">
                  HIPAA compliant with advanced security features, data encryption, 
                  and secure authentication mechanisms.
                </p>
              </div>

              <div className="bg-white rounded-2xl p-8 shadow-lg hover:shadow-xl transition-shadow border border-sky-100">
                <div className="w-12 h-12 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center mb-6">
                  <CpuChipIcon className="w-6 h-6 text-white" />
                </div>
                <h3 className="text-xl font-semibold text-gray-900 mb-3">Smart Automation</h3>
                <p className="text-gray-600">
                  Automated inventory management, prescription refill reminders, 
                  and intelligent stock optimization algorithms.
                </p>
              </div>
            </div>
          </div>
        </section>

        {/* CTA Section */}
        <section className="py-20 bg-gradient-to-r from-sky-500 to-blue-600">
          <div className="max-w-4xl mx-auto text-center px-4 sm:px-6 lg:px-8">
            <h2 className="text-3xl md:text-4xl font-bold text-white mb-6">
              Ready to Transform Your Pharmacy?
            </h2>
            <p className="text-xl text-sky-100 mb-8 leading-relaxed">
              Join thousands of healthcare professionals who trust CarePulse 
              to streamline their operations and improve patient care.
            </p>
            <Link 
              href="/login"
              className="inline-block bg-white text-sky-600 px-8 py-4 rounded-xl font-semibold text-lg hover:bg-gray-50 transition-colors shadow-lg hover:shadow-xl transform hover:scale-105"
            >
              Get Started Today
            </Link>
          </div>
        </section>
      </main>

      {/* Footer */}
      <footer className="bg-gray-900 text-white py-12">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex flex-col md:flex-row justify-between items-center">
            <div className="flex items-center space-x-2 mb-4 md:mb-0">
              <div className="w-8 h-8 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center">
                <HeartIcon className="w-5 h-5 text-white" />
              </div>
              <span className="text-xl font-bold">
                Care<span className="text-sky-400">Pulse</span>
              </span>
            </div>
            <div className="text-gray-400 text-sm">
              © 2025 CarePulse. All rights reserved. Built with ❤️ for healthcare professionals.
            </div>
          </div>
        </div>
      </footer>
    </div>
  );
}
