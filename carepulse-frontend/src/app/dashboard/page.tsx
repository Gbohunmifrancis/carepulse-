"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import {
  HeartIcon,
  Bars3Icon,
  XMarkIcon,
  HomeIcon,
  PlusIcon,
  ChartBarIcon,
  UserGroupIcon,
  Cog6ToothIcon,
  ArrowRightOnRectangleIcon,
  ClockIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  CurrencyDollarIcon,
  UsersIcon,
  ShoppingCartIcon,
  ArrowTrendingUpIcon,
  ArrowTrendingDownIcon,
  EyeIcon,
} from "@heroicons/react/24/outline";

// Mock data for demonstration - replace with actual API calls
const mockStats = {
  todaysSales: 95.00,
  salesGrowth: 4.9,
  availableComposites: 1457,
  compositesGrowth: 2.5,
  expiredMedicines: 0.00,
  expiredGrowth: -3.2,
  systemUsers: 255,
  usersGrowth: 2.9,
  totalSales: 298000,
  totalPurchases: 756000,
  totalSuppliers: 45,
  noSales: 12,
};

const mockRecentSales = [
  {
    id: 1,
    name: "Susan Williams",
    medicine: "Medicine Two",
    email: "gratis@emailhero.com",
    quantity: 1,
    totalPrice: 84.00,
    date: "Apr 23, 2025 12:00 AM",
  },
  {
    id: 2,
    name: "Berkley Howard",
    medicine: "Test Medicine",
    email: "gratis@emailhero.com",
    quantity: 1,
    totalPrice: 396.00,
    date: "Apr 22, 2025 12:00 AM",
  },
  {
    id: 3,
    name: "Evelyn Johnson",
    medicine: "Medicine One",
    email: "gratis@emailhero.com",
    quantity: 1,
    totalPrice: 220.00,
    date: "Apr 22, 2025 12:00 AM",
  },
];

const mockChartData = {
  purchases: 38,
  suppliers: 18,
  sales: 23,
  noSales: 21,
};

export default function DashboardPage() {
  const router = useRouter();
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [user, setUser] = useState({ name: "Dr. Sarah Wilson", role: "Pharmacist" });
  const [currentMonth, setCurrentMonth] = useState("This Month");

  useEffect(() => {
    // Check if user is authenticated
    const token = localStorage.getItem('token');
    if (!token) {
      router.push('/login');
    }
  }, [router]);

  const handleLogout = () => {
    localStorage.removeItem('token');
    router.push('/');
  };

  const navigation = [
    { name: 'Dashboard', href: '/dashboard', icon: HomeIcon, current: true },
    { name: 'Prescriptions', href: '/dashboard/prescriptions', icon: PlusIcon, current: false },
    { name: 'Inventory', href: '/dashboard/inventory', icon: ShoppingCartIcon, current: false },
    { name: 'Users', href: '/dashboard/users', icon: UserGroupIcon, current: false },
    { name: 'Settings', href: '/dashboard/settings', icon: Cog6ToothIcon, current: false },
  ];

  const StatCard = ({ title, value, growth, icon: Icon, color, prefix = "", suffix = "" }: {
    title: string;
    value: number | string;
    growth: number;
    icon: any;
    color: string;
    prefix?: string;
    suffix?: string;
  }) => {
    const isPositive = growth > 0;
    
    return (
      <div className={`${color} rounded-2xl p-6 relative overflow-hidden`}>
        <div className="flex items-center justify-between">
          <div className="flex-1">
            <p className="text-sm font-medium text-gray-700 mb-1">{title}</p>
            <p className="text-2xl font-bold text-gray-900">
              {prefix}{typeof value === 'number' ? value.toLocaleString() : value}{suffix}
            </p>
            <div className="flex items-center mt-2">
              {isPositive ? (
                <ArrowTrendingUpIcon className="w-4 h-4 text-green-500 mr-1" />
              ) : (
                <ArrowTrendingDownIcon className="w-4 h-4 text-red-500 mr-1" />
              )}
              <span className={`text-sm font-medium ${isPositive ? 'text-green-600' : 'text-red-600'}`}>
                {Math.abs(growth)}% This Month
              </span>
            </div>
          </div>
          <div className="ml-4">
            <div className="w-12 h-12 bg-white/20 rounded-lg flex items-center justify-center">
              <Icon className="w-6 h-6 text-gray-700" />
            </div>
          </div>
        </div>
        {/* Background decoration */}
        <div className="absolute -right-4 -top-4 w-24 h-24 bg-white/10 rounded-full"></div>
      </div>
    );
  };

  const DonutChart = ({ data, total, label }: {
    data: Record<string, number>;
    total: number;
    label: string;
  }) => {
    const colors = ['#10B981', '#3B82F6', '#F59E0B', '#EF4444'];
    const dataArray = Object.values(data);
    const labels = Object.keys(data);
    
    let cumulativePercentage = 0;
    const segments = dataArray.map((value, index) => {
      const percentage = (value / total) * 100;
      const startAngle = (cumulativePercentage / 100) * 360;
      const endAngle = ((cumulativePercentage + percentage) / 100) * 360;
      cumulativePercentage += percentage;
      
      return {
        value,
        percentage: Math.round(percentage),
        startAngle,
        endAngle,
        color: colors[index % colors.length],
        label: labels[index],
      };
    });

    return (
      <div className="relative w-48 h-48">
        <svg className="w-full h-full transform -rotate-90" viewBox="0 0 100 100">
          <circle
            cx="50"
            cy="50"
            r="40"
            fill="none"
            stroke="#F3F4F6"
            strokeWidth="10"
          />
          {segments.map((segment, index) => {
            const circumference = 2 * Math.PI * 40;
            const strokeDasharray = `${(segment.percentage / 100) * circumference} ${circumference}`;
            const strokeDashoffset = -((segment.startAngle / 360) * circumference);
            
            return (
              <circle
                key={index}
                cx="50"
                cy="50"
                r="40"
                fill="none"
                stroke={segment.color}
                strokeWidth="10"
                strokeDasharray={strokeDasharray}
                strokeDashoffset={strokeDashoffset}
                strokeLinecap="round"
              />
            );
          })}
        </svg>
        <div className="absolute inset-0 flex items-center justify-center">
          <div className="text-center">
            <div className="text-2xl font-bold text-gray-900">{total.toLocaleString()}</div>
            <div className="text-sm text-gray-500">Total</div>
          </div>
        </div>
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Mobile sidebar backdrop */}
      {sidebarOpen && (
        <div className="fixed inset-0 z-40 lg:hidden">
          <div
            className="fixed inset-0 bg-black bg-opacity-25"
            onClick={() => setSidebarOpen(false)}
          />
        </div>
      )}

      {/* Sidebar */}
      <div className={`fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-lg transform ${
        sidebarOpen ? 'translate-x-0' : '-translate-x-full'
      } transition-transform duration-200 ease-in-out lg:translate-x-0 lg:static lg:inset-0`}>
        <div className="flex items-center justify-between h-16 px-6 border-b border-gray-200">
          <Link href="/" className="flex items-center space-x-2">
            <div className="w-8 h-8 bg-gradient-to-r from-sky-500 to-blue-600 rounded-lg flex items-center justify-center">
              <HeartIcon className="w-5 h-5 text-white" />
            </div>
            <span className="text-xl font-bold text-gray-900">
              Care<span className="text-sky-600">Pulse</span>
            </span>
          </Link>
          <button
            onClick={() => setSidebarOpen(false)}
            className="lg:hidden"
          >
            <XMarkIcon className="w-6 h-6 text-gray-500" />
          </button>
        </div>

        <nav className="mt-6 px-3">
          <ul className="space-y-1">
            {navigation.map((item) => (
              <li key={item.name}>
                <Link
                  href={item.href}
                  className={`${
                    item.current
                      ? 'bg-sky-50 text-sky-700 border-r-2 border-sky-500'
                      : 'text-gray-600 hover:bg-gray-50 hover:text-gray-900'
                  } group flex items-center px-3 py-2 text-sm font-medium rounded-md transition-colors`}
                >
                  <item.icon
                    className={`${
                      item.current ? 'text-sky-500' : 'text-gray-400 group-hover:text-gray-500'
                    } mr-3 h-5 w-5`}
                  />
                  {item.name}
                </Link>
              </li>
            ))}
          </ul>
        </nav>

        <div className="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200">
          <div className="flex items-center space-x-3 mb-4">
            <div className="w-8 h-8 bg-sky-100 rounded-full flex items-center justify-center">
              <span className="text-sm font-medium text-sky-600">
                {user.name.split(' ').map(n => n[0]).join('')}
              </span>
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-900 truncate">{user.name}</p>
              <p className="text-xs text-gray-500">{user.role}</p>
            </div>
          </div>
          <button
            onClick={handleLogout}
            className="flex items-center w-full px-3 py-2 text-sm text-gray-600 hover:bg-gray-50 rounded-md transition-colors"
          >
            <ArrowRightOnRectangleIcon className="w-5 h-5 mr-3" />
            Sign out
          </button>
        </div>
      </div>

      {/* Main content */}
      <div className="lg:pl-64">
        {/* Top navigation */}
        <div className="bg-white shadow-sm border-b border-gray-200 px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex items-center">
              <button
                onClick={() => setSidebarOpen(true)}
                className="lg:hidden text-gray-500 hover:text-gray-700 mr-4"
              >
                <Bars3Icon className="w-6 h-6" />
              </button>
              <div>
                <h1 className="text-xl font-semibold text-gray-900">Welcome Code Astro!</h1>
                <p className="text-sm text-gray-500">Pharmacy Sales Results</p>
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <select 
                className="text-sm border border-gray-300 rounded-md px-3 py-1 focus:outline-none focus:ring-2 focus:ring-sky-500"
                value={currentMonth}
                onChange={(e) => setCurrentMonth(e.target.value)}
              >
                <option>This Month</option>
                <option>Last Month</option>
                <option>Last 3 Months</option>
              </select>
              <div className="flex items-center space-x-2">
                <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center">
                  <span className="text-xs text-white font-medium">BG</span>
                </div>
                <div className="text-right">
                  <p className="text-sm font-medium">Budiono Siregar</p>
                  <p className="text-xs text-gray-500">budiono.siregar@gmail.com</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Dashboard content */}
        <main className="p-6">
          {/* Stats cards */}
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
            <StatCard
              title="Today Sales"
              value={mockStats.todaysSales}
              growth={mockStats.salesGrowth}
              icon={CurrencyDollarIcon}
              color="bg-gradient-to-br from-green-100 to-green-200"
              prefix="$ "
            />
            <StatCard
              title="Available Composites"
              value={mockStats.availableComposites}
              growth={mockStats.compositesGrowth}
              icon={ShoppingCartIcon}
              color="bg-gradient-to-br from-blue-100 to-blue-200"
              suffix="%"
            />
            <StatCard
              title="Expired Medicines"
              value={mockStats.expiredMedicines}
              growth={mockStats.expiredGrowth}
              icon={ExclamationTriangleIcon}
              color="bg-gradient-to-br from-red-100 to-red-200"
              suffix="%"
            />
            <StatCard
              title="System Users"
              value={mockStats.systemUsers}
              growth={mockStats.usersGrowth}
              icon={UsersIcon}
              color="bg-gradient-to-br from-purple-100 to-purple-200"
              suffix="K"
            />
          </div>

          {/* Charts and table section */}
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Graph Report */}
            <div className="bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-semibold text-gray-900">Graph Report</h3>
                <button className="text-gray-400 hover:text-gray-600">
                  <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
                  </svg>
                </button>
              </div>
              
              <div className="flex justify-center mb-6">
                <DonutChart
                  data={mockChartData}
                  total={mockStats.totalPurchases / 1000}
                  label="Total"
                />
              </div>

              <div className="space-y-3">
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-2">
                    <div className="w-3 h-3 bg-green-500 rounded-full"></div>
                    <span className="text-sm text-gray-600">Purchases</span>
                  </div>
                  <span className="text-sm font-medium">{mockChartData.purchases}%</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-2">
                    <div className="w-3 h-3 bg-blue-500 rounded-full"></div>
                    <span className="text-sm text-gray-600">Suppliers</span>
                  </div>
                  <span className="text-sm font-medium">{mockChartData.suppliers}%</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-2">
                    <div className="w-3 h-3 bg-yellow-500 rounded-full"></div>
                    <span className="text-sm text-gray-600">Sales</span>
                  </div>
                  <span className="text-sm font-medium">{mockChartData.sales}%</span>
                </div>
                <div className="flex items-center justify-between">
                  <div className="flex items-center space-x-2">
                    <div className="w-3 h-3 bg-red-500 rounded-full"></div>
                    <span className="text-sm text-gray-600">No Sales</span>
                  </div>
                  <span className="text-sm font-medium">{mockChartData.noSales}%</span>
                </div>
              </div>
            </div>

            {/* Total Sales Overview */}
            <div className="lg:col-span-2 bg-white rounded-2xl shadow-sm border border-gray-200 p-6">
              <div className="flex items-center justify-between mb-6">
                <h3 className="text-lg font-semibold text-gray-900">Total Sales Overview</h3>
                <button className="text-gray-400 hover:text-gray-600">
                  <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                    <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
                  </svg>
                </button>
              </div>

              <div className="mb-4">
                <div className="text-2xl font-bold text-gray-900">
                  ${mockStats.totalSales.toLocaleString()}K
                </div>
              </div>

              <div className="grid grid-cols-7 gap-2 h-48">
                {['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((day, index) => {
                  const heights = [60, 80, 90, 75, 85, 70];
                  const colors = ['#FF6B6B', '#4ECDC4', '#45B7D1', '#96CEB4', '#FECA57', '#FF9FF3'];
                  
                  return (
                    <div key={day} className="flex flex-col items-center">
                      <div className="flex-1 flex items-end mb-2">
                        <div
                          className="w-8 rounded-t-md"
                          style={{
                            height: `${heights[index]}%`,
                            backgroundColor: colors[index],
                          }}
                        ></div>
                      </div>
                      <span className="text-xs text-gray-500">{day}</span>
                    </div>
                  );
                })}
              </div>
            </div>
          </div>

          {/* Recent Sales List */}
          <div className="mt-8 bg-white rounded-2xl shadow-sm border border-gray-200">
            <div className="px-6 py-4 border-b border-gray-200">
              <div className="flex items-center justify-between">
                <h3 className="text-lg font-semibold text-gray-900">Recent Sales List</h3>
                <div className="flex items-center space-x-4">
                  <button className="text-sm text-gray-600 hover:text-gray-900">Search...</button>
                  <button className="text-sm text-gray-600 hover:text-gray-900">Filter ↓</button>
                  <button className="text-sm text-gray-600 hover:text-gray-900">Sort By ↓</button>
                  <button className="text-gray-400 hover:text-gray-600">
                    <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M10 6a2 2 0 110-4 2 2 0 010 4zM10 12a2 2 0 110-4 2 2 0 010 4zM10 18a2 2 0 110-4 2 2 0 010 4z" />
                    </svg>
                  </button>
                </div>
              </div>
            </div>

            <div className="overflow-x-auto">
              <table className="w-full">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      <input type="checkbox" className="rounded" />
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Name
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Medicine
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      User Email
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Quantity
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Total Price
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Date
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Actions
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {mockRecentSales.map((sale) => (
                    <tr key={sale.id} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <input type="checkbox" className="rounded" />
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="w-8 h-8 bg-orange-100 rounded-full flex items-center justify-center mr-3">
                            <span className="text-xs font-medium text-orange-600">
                              {sale.name.split(' ').map(n => n[0]).join('')}
                            </span>
                          </div>
                          <span className="text-sm font-medium text-gray-900">{sale.name}</span>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {sale.medicine}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {sale.email}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className="w-6 h-6 bg-gray-900 text-white rounded-full flex items-center justify-center text-xs">
                          {sale.quantity}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                        ${sale.totalPrice.toFixed(2)}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        {sale.date}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center space-x-2">
                          <button className="text-blue-600 hover:text-blue-900">
                            <EyeIcon className="w-4 h-4" />
                          </button>
                          <button className="text-green-600 hover:text-green-900">
                            ✓
                          </button>
                          <span className="text-gray-400">...</span>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="px-6 py-4 border-t border-gray-200">
              <div className="flex items-center justify-between">
                <div className="text-sm text-gray-700">
                  Showing 1 of 200 entries
                </div>
                <div className="flex items-center space-x-2">
                  <button className="px-3 py-1 text-sm border border-gray-300 rounded hover:bg-gray-50">
                    Prev
                  </button>
                  <button className="px-3 py-1 text-sm bg-gray-900 text-white rounded">
                    1
                  </button>
                  <button className="px-3 py-1 text-sm border border-gray-300 rounded hover:bg-gray-50">
                    2
                  </button>
                  <span className="text-sm text-gray-500">...</span>
                  <button className="px-3 py-1 text-sm border border-gray-300 rounded hover:bg-gray-50">
                    Next
                  </button>
                </div>
                <div className="text-sm text-gray-700">
                  Show 5 ↓
                </div>
              </div>
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}