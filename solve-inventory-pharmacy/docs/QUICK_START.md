# 🚀 Quick Start Guide for New Developers

## Prerequisites Checklist
- [ ] .NET 8 SDK installed
- [ ] Git configured
- [ ] Entity Framework tools: `dotnet tool install --global dotnet-ef`

## ⚡ Quick Setup (5 minutes)

### 1. Clone and Setup
```bash
git clone <repository-url>
cd solve-inventory-pharmacy
dotnet restore
```

### 2. Database Setup
```bash
dotnet ef database update --project src/SolveStation.Data --startup-project src/SolveStation.PharmacyApi
```

### 3. Run the API
```bash
dotnet run --project src/SolveStation.PharmacyApi
```

### 4. Access Swagger UI
Open browser: `http://localhost:5000`

## 🛠️ If Something Goes Wrong

### API Won't Start?
```bash
# Check environment
echo $env:ASPNETCORE_ENVIRONMENT  # Should be "Development"

# Restore packages
dotnet restore src/SolveStation.PharmacyApi
```

### Swagger UI Not Loading?
- ✅ Ensure `ASPNETCORE_ENVIRONMENT=Development`
- ✅ Check `Properties/launchSettings.json` exists
- ✅ Access via `http://localhost:5000` (not https)

### Database Errors?
```bash
# Reset database
dotnet ef database drop --project src/SolveStation.Data --startup-project src/SolveStation.PharmacyApi --force
dotnet ef database update --project src/SolveStation.Data --startup-project src/SolveStation.PharmacyApi
```

## 📋 What's Implemented
- ✅ Complete REST API with 5 controllers
- ✅ JWT Authentication & Role-based Authorization  
- ✅ Swagger UI Documentation
- ✅ Entity Framework with SQL Server
- ✅ Comprehensive Validation
- ✅ Error Handling Middleware
- ✅ Health Check Endpoints

## 🎯 Ready to Code!
- **API Docs**: `http://localhost:5000`
- **Health Check**: `http://localhost:5000/api/health`
- **Sample Endpoints**: See Swagger UI for interactive testing

📖 **For detailed information**, see [API_IMPLEMENTATION_GUIDE.md](./API_IMPLEMENTATION_GUIDE.md)
