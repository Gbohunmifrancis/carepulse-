import { NextRequest, NextResponse } from 'next/server';

export async function GET(request: NextRequest) {
  try {
    const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:5000/api';
    
    console.log('Attempting to connect to:', `${API_BASE_URL}/health`);
    
    // Test connection to your .NET API health endpoint
    const response = await fetch(`${API_BASE_URL}/health`, {
      method: 'GET',
      headers: {
        'Accept': 'application/json',
      },
    });

    console.log('Response status:', response.status);
    console.log('Response headers:', Object.fromEntries(response.headers.entries()));

    if (response.ok) {
      const data = await response.text();
      return NextResponse.json({ 
        success: true, 
        message: 'Successfully connected to .NET API',
        data: data,
        apiUrl: `${API_BASE_URL}/health`
      });
    } else {
      return NextResponse.json({ 
        success: false, 
        message: `API returned status: ${response.status}`,
        apiUrl: `${API_BASE_URL}/health`
      }, { status: response.status });
    }
  } catch (error) {
    console.error('API connection test error:', error);
    return NextResponse.json({ 
      success: false, 
      message: `Connection failed: ${error instanceof Error ? error.message : 'Unknown error'}`,
      apiUrl: process.env.API_BASE_URL || 'http://localhost:5000/api'
    }, { status: 500 });
  }
}