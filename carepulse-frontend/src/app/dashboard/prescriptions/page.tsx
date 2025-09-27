"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
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
  MagnifyingGlassIcon,
  FunnelIcon,
} from "@heroicons/react/24/outline";
import { apiClient } from "@/lib/api";

// Mock data - replace with actual API calls
const mockPrescriptions = [
  {
    id: "RX001",
    patientName: "John Smith",
    patientEmail: "john.smith@email.com",
    doctorName: "Dr. Sarah Williams",
    status: "pending",
    items: [
      { drugName: "Amoxicillin 500mg", quantity: 30, instructions: "Take one capsule twice daily" },
      { drugName: "Ibuprofen 400mg", quantity: 20, instructions: "Take as needed for pain" }
    ],
    notes: "Patient has mild penicillin allergy - monitor for reactions",
    createdAt: "2025-01-15T10:30:00Z",
    updatedAt: "2025-01-15T10:30:00Z"
  },
  {
    id: "RX002",
    patientName: "Sarah Johnson",
    patientEmail: "sarah.johnson@email.com",
    doctorName: "Dr. Michael Brown",
    status: "completed",
    items: [
      { drugName: "Lisinopril 10mg", quantity: 30, instructions: "Take once daily in the morning" }
    ],
    notes: "",
    createdAt: "2025-01-15T09:15:00Z",
    updatedAt: "2025-01-15T11:45:00Z"
  },
  {
    id: "RX003",
    patientName: "Michael Davis",
    patientEmail: "michael.davis@email.com",
    doctorName: "Dr. Emily Wilson",
    status: "pending",
    items: [
      { drugName: "Metformin 850mg", quantity: 60, instructions: "Take with meals twice daily" },
      { drugName: "Glipizide 5mg", quantity: 30, instructions: "Take once daily before breakfast" }
    ],
    notes: "Diabetic patient - regular monitoring required",
    createdAt: "2025-01-15T08:45:00Z",
    updatedAt: "2025-01-15T08:45:00Z"
  },
  {
    id: "RX004",
    patientName: "Emma Thompson",
    patientEmail: "emma.thompson@email.com",
    doctorName: "Dr. James Miller",
    status: "completed",
    items: [
      { drugName: "Levothyroxine 50mcg", quantity: 30, instructions: "Take on empty stomach, 1 hour before breakfast" }
    ],
    notes: "Thyroid medication - consistent timing important",
    createdAt: "2025-01-14T16:20:00Z",
    updatedAt: "2025-01-15T09:10:00Z"
  },
  {
    id: "RX005",
    patientName: "Robert Chen",
    patientEmail: "robert.chen@email.com",
    doctorName: "Dr. Lisa Anderson",
    status: "cancelled",
    items: [
      { drugName: "Atorvastatin 20mg", quantity: 30, instructions: "Take once daily in the evening" }
    ],
    notes: "Cancelled due to drug interaction concerns",
    createdAt: "2025-01-14T14:30:00Z",
    updatedAt: "2025-01-14T15:45:00Z"
  },
];

export default function PrescriptionsPage() {
  const [sidebarOpen, setSidebarOpen] = useState(false);
  const [prescriptions, setPrescriptions] = useState(mockPrescriptions);
  const [searchTerm, setSearchTerm] = useState("");
  const [statusFilter, setStatusFilter] = useState("all");
  const [isLoading, setIsLoading] = useState(false);

  const navigation = [
    { name: 'Dashboard', href: '/dashboard', icon: HomeIcon, current: false },
    { name: 'Prescriptions', href: '/dashboard/prescriptions', icon: PlusIcon, current: true },
    { name: 'Inventory', href: '/dashboard/inventory', icon: ChartBarIcon, current: false },
    { name: 'Users', href: '/dashboard/users', icon: UserGroupIcon, current: false },
    { name: 'Settings', href: '/dashboard/settings', icon: Cog6ToothIcon, current: false },
  ];

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'completed':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'pending':
        return 'bg-yellow-100 text-yellow-800 border-yellow-200';
      case 'cancelled':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'completed':
        return <CheckCircleIcon className="w-4 h-4" />;
      case 'pending':
        return <ClockIcon className="w-4 h-4" />;
      case 'cancelled':
        return <XMarkIcon className="w-4 h-4" />;
      default:
        return <ClockIcon className="w-4 h-4" />;
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const filteredPrescriptions = prescriptions.filter(prescription => {
    const matchesSearch = prescription.patientName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         prescription.doctorName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         prescription.id.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesStatus = statusFilter === "all" || prescription.status === statusFilter;
    
    return matchesSearch && matchesStatus;
  });

  const handleStatusUpdate = async (prescriptionId: string, newStatus: string) => {
    try {
      setIsLoading(true);
      // In real implementation, call API here
      // await apiClient.updatePrescriptionStatus(prescriptionId, newStatus);
      
      setPrescriptions(prev => prev.map(prescription =>
        prescription.id === prescriptionId 
          ? { ...prescription, status: newStatus, updatedAt: new Date().toISOString() }
          : prescription
      ));
    } catch (error) {
      console.error('Failed to update prescription status:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('token');
    window.location.href = '/';
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
              <span className="text-sm font-medium text-sky-600">DR</span>
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-900 truncate">Dr. Sarah Wilson</p>
              <p className="text-xs text-gray-500">Pharmacist</p>
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
                className="lg:hidden text-gray-500 hover:text-gray-700"
              >
                <Bars3Icon className="w-6 h-6" />
              </button>
              <h1 className="ml-4 lg:ml-0 text-xl font-semibold text-gray-900">Prescriptions</h1>
            </div>
            <div className="flex items-center space-x-4">
              <button className="bg-sky-600 text-white px-4 py-2 rounded-lg hover:bg-sky-700 transition-colors">
                New Prescription
              </button>
            </div>
          </div>
        </div>

        {/* Page content */}
        <main className="p-4 sm:p-6 lg:p-8">
          {/* Filters */}
          <div className="mb-6 bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Search */}
              <div className="relative">
                <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Search prescriptions..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                />
              </div>

              {/* Status filter */}
              <div className="relative">
                <FunnelIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
                <select
                  value={statusFilter}
                  onChange={(e) => setStatusFilter(e.target.value)}
                  className="pl-10 w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-sky-500 focus:border-transparent"
                >
                  <option value="all">All Statuses</option>
                  <option value="pending">Pending</option>
                  <option value="completed">Completed</option>
                  <option value="cancelled">Cancelled</option>
                </select>
              </div>

              {/* Results count */}
              <div className="flex items-center text-sm text-gray-600">
                Showing {filteredPrescriptions.length} of {prescriptions.length} prescriptions
              </div>
            </div>
          </div>

          {/* Prescriptions List */}
          <div className="space-y-4">
            {filteredPrescriptions.map((prescription) => (
              <div key={prescription.id} className="bg-white rounded-xl shadow-sm border border-gray-200">
                <div className="p-6">
                  <div className="flex items-start justify-between">
                    <div className="flex-1">
                      {/* Header */}
                      <div className="flex items-center space-x-4 mb-4">
                        <div className="flex items-center space-x-2">
                          {getStatusIcon(prescription.status)}
                          <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${getStatusColor(prescription.status)}`}>
                            {prescription.status}
                          </span>
                        </div>
                        <span className="text-sm font-mono text-gray-500">#{prescription.id}</span>
                        <span className="text-sm text-gray-500">{formatDate(prescription.createdAt)}</span>
                      </div>

                      {/* Patient and Doctor info */}
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-4">
                        <div>
                          <p className="text-sm font-medium text-gray-500">Patient</p>
                          <p className="text-lg font-semibold text-gray-900">{prescription.patientName}</p>
                          <p className="text-sm text-gray-600">{prescription.patientEmail}</p>
                        </div>
                        <div>
                          <p className="text-sm font-medium text-gray-500">Prescribing Doctor</p>
                          <p className="text-lg font-semibold text-gray-900">{prescription.doctorName}</p>
                        </div>
                      </div>

                      {/* Medications */}
                      <div className="mb-4">
                        <p className="text-sm font-medium text-gray-500 mb-2">Medications</p>
                        <div className="space-y-2">
                          {prescription.items.map((item, index) => (
                            <div key={index} className="flex items-center justify-between bg-gray-50 rounded-lg p-3">
                              <div className="flex-1">
                                <p className="font-medium text-gray-900">{item.drugName}</p>
                                <p className="text-sm text-gray-600">{item.instructions}</p>
                              </div>
                              <div className="text-right">
                                <p className="text-sm font-medium text-gray-900">Qty: {item.quantity}</p>
                              </div>
                            </div>
                          ))}
                        </div>
                      </div>

                      {/* Notes */}
                      {prescription.notes && (
                        <div className="mb-4">
                          <p className="text-sm font-medium text-gray-500 mb-1">Notes</p>
                          <p className="text-sm text-gray-700 bg-yellow-50 border border-yellow-200 rounded-lg p-3">
                            {prescription.notes}
                          </p>
                        </div>
                      )}
                    </div>

                    {/* Actions */}
                    <div className="ml-6 flex flex-col space-y-2">
                      {prescription.status === 'pending' && (
                        <>
                          <button
                            onClick={() => handleStatusUpdate(prescription.id, 'completed')}
                            disabled={isLoading}
                            className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50 text-sm font-medium transition-colors"
                          >
                            Complete
                          </button>
                          <button
                            onClick={() => handleStatusUpdate(prescription.id, 'cancelled')}
                            disabled={isLoading}
                            className="px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 disabled:opacity-50 text-sm font-medium transition-colors"
                          >
                            Cancel
                          </button>
                        </>
                      )}
                      <button className="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700 text-sm font-medium transition-colors">
                        View Details
                      </button>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>

          {filteredPrescriptions.length === 0 && (
            <div className="text-center py-12">
              <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                <PlusIcon className="w-8 h-8 text-gray-400" />
              </div>
              <h3 className="text-lg font-medium text-gray-900 mb-2">No prescriptions found</h3>
              <p className="text-gray-600 mb-6">Try adjusting your search terms or filters.</p>
              <button className="bg-sky-600 text-white px-6 py-2 rounded-lg hover:bg-sky-700 transition-colors">
                Create New Prescription
              </button>
            </div>
          )}
        </main>
      </div>
    </div>
  );
}