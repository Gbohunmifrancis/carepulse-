import { NextRequest, NextResponse } from 'next/server';

const API_BASE_URL = process.env.API_BASE_URL || 'http://localhost:5000/api';

export async function POST(request: NextRequest) {
  try {
    const body = await request.json();
    
    console.log('Login attempt for:', body.email);
    console.log('API URL:', `${API_BASE_URL}/auth/login`);
    
    // Call your .NET API
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      body: JSON.stringify(body),
    });

    console.log('API Response Status:', response.status);
    console.log('API Response Headers:', Object.fromEntries(response.headers.entries()));

    const responseText = await response.text();
    console.log('API Response Body:', responseText);

    let data;
    try {
      data = JSON.parse(responseText);
    } catch (parseError) {
      console.error('Failed to parse API response as JSON:', parseError);
      return NextResponse.json(
        { message: `API returned invalid JSON: ${responseText.substring(0, 200)}` },
        { status: 502 }
      );
    }

    if (response.ok) {
      return NextResponse.json(data);
    } else {
      return NextResponse.json(
        { message: data.message || data.title || 'Login failed' },
        { status: response.status }
      );
    }
  } catch (error) {
    console.error('Login API error:', error);
    return NextResponse.json(
      { 
        message: `Internal server error: ${error instanceof Error ? error.message : 'Unknown error'}`,
        details: error instanceof Error ? error.stack : 'No stack trace available'
      },
      { status: 500 }
    );
  }
}