"use client";

import { useState } from 'react';

export default function TestPage() {
  const [testResult, setTestResult] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(false);

  const testConnection = async () => {
    setIsLoading(true);
    try {
      const response = await fetch('/api/test');
      const data = await response.json();
      setTestResult(data);
    } catch (error) {
      setTestResult({ 
        success: false, 
        message: `Frontend error: ${error instanceof Error ? error.message : 'Unknown error'}` 
      });
    } finally {
      setIsLoading(false);
    }
  };

  const testLogin = async () => {
    setIsLoading(true);
    try {
      const response = await fetch('/api/auth/login', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          email: 'admin@carepulse.com',
          password: 'admin123'
        }),
      });
      const data = await response.json();
      setTestResult({ ...data, endpoint: 'login' });
    } catch (error) {
      setTestResult({ 
        success: false, 
        message: `Login test error: ${error instanceof Error ? error.message : 'Unknown error'}`,
        endpoint: 'login'
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-12 px-4">
      <div className="max-w-4xl mx-auto">
        <h1 className="text-3xl font-bold text-gray-900 mb-8">CarePulse API Connection Test</h1>
        
        <div className="bg-white rounded-xl shadow p-6 mb-6">
          <h2 className="text-xl font-semibold mb-4">Test API Connection</h2>
          <div className="flex space-x-4 mb-6">
            <button
              onClick={testConnection}
              disabled={isLoading}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
            >
              {isLoading ? 'Testing...' : 'Test Health Endpoint'}
            </button>
            <button
              onClick={testLogin}
              disabled={isLoading}
              className="px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 disabled:opacity-50"
            >
              {isLoading ? 'Testing...' : 'Test Login Endpoint'}
            </button>
          </div>
          
          {testResult && (
            <div className="border rounded-lg p-4 bg-gray-50">
              <h3 className="font-semibold mb-2">Test Result:</h3>
              <div className="space-y-2">
                <div className={`inline-flex items-center px-3 py-1 rounded-full text-sm font-medium ${
                  testResult.success 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-red-100 text-red-800'
                }`}>
                  {testResult.success ? '✅ Success' : '❌ Failed'}
                </div>
                {testResult.endpoint && (
                  <p><strong>Endpoint:</strong> {testResult.endpoint}</p>
                )}
                {testResult.apiUrl && (
                  <p><strong>API URL:</strong> {testResult.apiUrl}</p>
                )}
                <p><strong>Message:</strong> {testResult.message}</p>
                {testResult.data && (
                  <div>
                    <strong>Response Data:</strong>
                    <pre className="bg-gray-100 p-2 rounded text-sm overflow-x-auto">
                      {JSON.stringify(testResult.data, null, 2)}
                    </pre>
                  </div>
                )}
                {testResult.details && (
                  <div>
                    <strong>Error Details:</strong>
                    <pre className="bg-gray-100 p-2 rounded text-sm overflow-x-auto">
                      {testResult.details}
                    </pre>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>

        <div className="bg-white rounded-xl shadow p-6">
          <h2 className="text-xl font-semibold mb-4">Configuration Information</h2>
          <div className="space-y-2 text-sm">
            <p><strong>Expected API Base URL:</strong> http://localhost:5000/api</p>
            <p><strong>Frontend URL:</strong> http://localhost:3000</p>
            <p><strong>Test Endpoints:</strong></p>
            <ul className="list-disc list-inside ml-4 space-y-1">
              <li>Health: http://localhost:5000/api/health</li>
              <li>Login: http://localhost:5000/api/auth/login</li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
}