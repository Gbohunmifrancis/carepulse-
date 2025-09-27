// API configuration and utilities for CarePulse frontend

export const API_CONFIG = {
  BASE_URL: process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api',
  TIMEOUT: 30000,
};

// API client utility
class ApiClient {
  private baseURL: string;
  private defaultHeaders: Record<string, string>;

  constructor(baseURL: string) {
    this.baseURL = baseURL;
    this.defaultHeaders = {
      'Content-Type': 'application/json',
      'Accept': 'application/json',
    };
  }

  private getAuthHeader(): Record<string, string> {
    if (typeof window !== 'undefined') {
      const token = localStorage.getItem('token');
      return token ? { 'Authorization': `Bearer ${token}` } : {};
    }
    return {};
  }

  private async request<T>(
    endpoint: string,
    options: RequestInit = {}
  ): Promise<T> {
    const url = `${this.baseURL}${endpoint}`;
    const headers = {
      ...this.defaultHeaders,
      ...this.getAuthHeader(),
      ...options.headers,
    };

    const response = await fetch(url, {
      ...options,
      headers,
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({}));
      throw new Error(error.message || `HTTP error! status: ${response.status}`);
    }

    return response.json();
  }

  // Authentication endpoints
  async login(credentials: { email: string; password: string }) {
    return this.request<{ token: string; user: any }>('/auth/login', {
      method: 'POST',
      body: JSON.stringify(credentials),
    });
  }

  async refreshToken(refreshToken: string) {
    return this.request<{ token: string }>('/auth/refresh', {
      method: 'POST',
      body: JSON.stringify({ refreshToken }),
    });
  }

  async getCurrentUser() {
    return this.request<any>('/auth/me');
  }

  // Drug/Inventory endpoints
  async getDrugs(params: {
    page?: number;
    pageSize?: number;
    search?: string;
    lowStock?: boolean;
  } = {}) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined) {
        query.append(key, value.toString());
      }
    });
    
    return this.request<{
      data: any[];
      totalCount: number;
      page: number;
      pageSize: number;
    }>(`/drugs?${query}`);
  }

  async getDrug(id: string) {
    return this.request<any>(`/drugs/${id}`);
  }

  async createDrug(drug: any) {
    return this.request<any>('/drugs', {
      method: 'POST',
      body: JSON.stringify(drug),
    });
  }

  async updateDrug(id: string, drug: any) {
    return this.request<any>(`/drugs/${id}`, {
      method: 'PUT',
      body: JSON.stringify(drug),
    });
  }

  async updateDrugStock(id: string, stockUpdate: { newQuantity: number; reason: string }) {
    return this.request<any>(`/drugs/${id}/stock`, {
      method: 'PATCH',
      body: JSON.stringify(stockUpdate),
    });
  }

  // Prescription endpoints
  async getPrescriptions(params: {
    page?: number;
    pageSize?: number;
    status?: string;
    patientName?: string;
  } = {}) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined) {
        query.append(key, value.toString());
      }
    });
    
    return this.request<{
      data: any[];
      totalCount: number;
      page: number;
      pageSize: number;
    }>(`/prescriptions?${query}`);
  }

  async getPrescription(id: string) {
    return this.request<any>(`/prescriptions/${id}`);
  }

  async createPrescription(prescription: any) {
    return this.request<any>('/prescriptions', {
      method: 'POST',
      body: JSON.stringify(prescription),
    });
  }

  async updatePrescriptionStatus(id: string, status: string) {
    return this.request<any>(`/prescriptions/${id}/status`, {
      method: 'PATCH',
      body: JSON.stringify({ status }),
    });
  }

  // User management endpoints
  async getUsers(params: {
    page?: number;
    pageSize?: number;
    role?: string;
  } = {}) {
    const query = new URLSearchParams();
    Object.entries(params).forEach(([key, value]) => {
      if (value !== undefined) {
        query.append(key, value.toString());
      }
    });
    
    return this.request<{
      data: any[];
      totalCount: number;
      page: number;
      pageSize: number;
    }>(`/users?${query}`);
  }

  async getUser(id: string) {
    return this.request<any>(`/users/${id}`);
  }

  async createUser(user: any) {
    return this.request<any>('/users', {
      method: 'POST',
      body: JSON.stringify(user),
    });
  }

  async updateUser(id: string, user: any) {
    return this.request<any>(`/users/${id}`, {
      method: 'PUT',
      body: JSON.stringify(user),
    });
  }

  async changePassword(id: string, passwordData: { currentPassword: string; newPassword: string }) {
    return this.request<any>(`/users/${id}/change-password`, {
      method: 'POST',
      body: JSON.stringify(passwordData),
    });
  }
}

// Export singleton instance
export const apiClient = new ApiClient(API_CONFIG.BASE_URL);

// Type definitions for API responses
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  role: string;
  lastLoginAt?: string;
  isEmailVerified: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface Drug {
  id: string;
  name: string;
  description: string;
  price: number;
  stockQuantity: number;
  minimumStockLevel: number;
  expiryDate: string;
  supplier: string;
  batchNumber?: string;
  drugCode?: string;
  isLowStock: boolean;
  isExpired: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface Prescription {
  id: string;
  patientName: string;
  patientEmail: string;
  doctorId: string;
  doctorName: string;
  status: 'pending' | 'completed' | 'cancelled';
  items: PrescriptionItem[];
  notes?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface PrescriptionItem {
  id: string;
  prescriptionId: string;
  drugId: string;
  drugName: string;
  quantity: number;
  instructions: string;
}

export interface ApiResponse<T> {
  data: T;
  totalCount?: number;
  page?: number;
  pageSize?: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
}

export interface ApiError {
  message: string;
  errors?: Record<string, string[]>;
}