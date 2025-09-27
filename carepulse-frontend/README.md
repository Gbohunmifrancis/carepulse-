# CarePulse Frontend

A modern, responsive healthcare management system frontend built with Next.js 14, TypeScript, and Tailwind CSS. This application provides a beautiful, intuitive interface for pharmacy management, prescription tracking, and inventory control.

## 🚀 Features

### 🏠 Landing Page
- **Beautiful Hero Section** with animated background elements
- **Feature Showcase** highlighting key capabilities
- **Responsive Design** that works on all devices
- **Sky Blue Theme** with professional healthcare aesthetics
- **Call-to-Action** sections driving user engagement

### 🔐 Authentication
- **Secure Login System** with JWT token management
- **Demo Credentials** for testing different user roles
- **Password Visibility Toggle** for better UX
- **Form Validation** with error handling
- **Auto-redirect** to dashboard after successful login

### 📊 Dashboard
- **Interactive Sidebar Navigation** with role-based access
- **Statistics Cards** showing key metrics
- **Recent Prescriptions** overview
- **Low Stock Alerts** with actionable items
- **Real-time Updates** and responsive design

### 💊 Prescription Management
- **Complete CRUD Operations** for prescriptions
- **Status Management** (Pending, Completed, Cancelled)
- **Advanced Search and Filtering** capabilities
- **Detailed Medication Lists** with instructions
- **Doctor and Patient Information** tracking

### 🏥 Inventory Control (Planned)
- Real-time stock monitoring
- Automated reorder alerts
- Expiry date tracking
- Supplier management

### 👥 User Management (Planned)
- Role-based access control
- User profile management
- Activity logging

## 🛠 Technology Stack

- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript
- **Styling**: Tailwind CSS
- **Icons**: Heroicons
- **HTTP Client**: Fetch API with custom wrapper
- **State Management**: React Hooks
- **Authentication**: JWT tokens with localStorage

## 🎨 Design System

### Color Palette
- **Primary**: Sky Blue (`#0ea5e9`) to Blue (`#2563eb`)
- **Secondary**: Cyan and various blue shades
- **Success**: Green (`#059669`)
- **Warning**: Yellow (`#d97706`)
- **Error**: Red (`#dc2626`)
- **Neutral**: Gray scale

### Typography
- **Font**: Inter (Google Fonts)
- **Headings**: Bold weights with proper hierarchy
- **Body**: Regular weight with good line height

## 🚀 Getting Started

### Prerequisites
- Node.js 18+ 
- npm or yarn
- Your .NET API running on `http://localhost:5000`

### Installation

1. **Navigate to the frontend directory**:
   ```bash
   cd carepulse-frontend
   ```

2. **Install dependencies**:
   ```bash
   npm install
   ```

3. **Set up environment variables**:
   ```bash
   # Copy .env.local and update if needed
   # The API URL should point to your .NET backend
   NEXT_PUBLIC_API_URL=http://localhost:5000/api
   ```

4. **Run the development server**:
   ```bash
   npm run dev
   ```

5. **Open your browser**:
   Visit [http://localhost:3000](http://localhost:3000)

## 📂 Project Structure

```
carepulse-frontend/
├── src/
│   ├── app/                    # App Router pages
│   │   ├── (auth)/
│   │   │   └── login/         # Authentication pages
│   │   ├── dashboard/         # Protected dashboard pages
│   │   │   ├── prescriptions/ # Prescription management
│   │   │   ├── inventory/     # Inventory management
│   │   │   └── users/         # User management
│   │   ├── api/               # API route handlers
│   │   ├── globals.css        # Global styles
│   │   ├── layout.tsx         # Root layout
│   │   └── page.tsx           # Landing page
│   └── lib/                   # Utilities and configurations
│       └── api.ts             # API client and types
├── public/                    # Static assets
├── .env.local                 # Environment variables
├── tailwind.config.js         # Tailwind configuration
└── package.json
```

## 🔗 API Integration

### Backend Connection
The frontend connects to your .NET API at `http://localhost:5000/api` with the following endpoints:

#### Authentication
- `POST /auth/login` - User authentication
- `POST /auth/refresh` - Token refresh
- `GET /auth/me` - Current user info

#### Prescriptions
- `GET /prescriptions` - List prescriptions with pagination
- `POST /prescriptions` - Create new prescription
- `GET /prescriptions/{id}` - Get prescription details
- `PATCH /prescriptions/{id}/status` - Update prescription status

#### Drugs/Inventory
- `GET /drugs` - List drugs with search and filters
- `POST /drugs` - Add new drug
- `PUT /drugs/{id}` - Update drug
- `PATCH /drugs/{id}/stock` - Update stock levels

#### Users
- `GET /users` - List users (Admin only)
- `POST /users` - Create user (Admin only)
- `PUT /users/{id}` - Update user
- `POST /users/{id}/change-password` - Change password

## 🎯 Demo Credentials

For testing purposes, you can use these demo accounts:

- **Admin**: `admin@carepulse.com` / `admin123`
- **Doctor**: `doctor@carepulse.com` / `doctor123`  
- **Pharmacist**: `pharmacist@carepulse.com` / `pharma123`

## 📱 Responsive Design

The application is fully responsive and optimized for:
- **Desktop**: Full sidebar navigation and multi-column layouts
- **Tablet**: Collapsible sidebar with adaptive grids
- **Mobile**: Drawer navigation and stacked layouts

## 🎨 UI Components

### Custom Components
- **Animated Backgrounds**: Blob animations for visual appeal
- **Status Badges**: Color-coded prescription statuses
- **Loading States**: Spinners and disabled states
- **Empty States**: Helpful messaging when no data
- **Search & Filters**: Real-time filtering capabilities

### Accessibility
- Proper ARIA labels and roles
- Keyboard navigation support
- Color contrast compliance
- Screen reader friendly

## 🔒 Security Features

- **JWT Token Management**: Automatic token storage and refresh
- **Route Protection**: Authenticated routes with redirects
- **Input Validation**: Client-side form validation
- **Error Handling**: Graceful error states and messaging

## 🚀 Deployment

### Build for Production
```bash
npm run build
```

### Deploy Options
- **Vercel**: Seamless Next.js deployment
- **Netlify**: Static site hosting
- **Docker**: Containerized deployment
- **Traditional Hosting**: Static export option

## 🤝 Contributing

1. Follow the existing code structure and patterns
2. Use TypeScript for all new components
3. Follow Tailwind CSS utilities for styling
4. Ensure responsive design for all new features
5. Test on multiple devices and browsers

## 📝 License

This project is part of the SolveStation Pharmacy Management System.

---

**Built with ❤️ for healthcare professionals**

For questions or support, please refer to the main project documentation.
