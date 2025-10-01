# Environment Variables Setup

This document explains how to set up environment variables for the CouponHub API.

## Environment Variables Required

| Variable | Description | Default Value |
|----------|-------------|---------------|
| `DB_CONNECTION_STRING` | PostgreSQL database connection string | `Host=localhost;Database=COUPONHUB;Username=postgres;Password=123456` |
| `JWT_KEY` | JWT secret key for token signing | `super_secret_key_change_in_production` |
| `JWT_ISSUER` | JWT issuer claim | `couponhub` |
| `JWT_AUDIENCE` | JWT audience claim | `couponhub_clients` |
| `JWT_EXPIRY_MINUTES` | JWT token expiry time in minutes | `120` |
| `LOG_LEVEL` | Application logging level | `Information` |
| `ALLOWED_HOSTS` | Allowed hosts for CORS | `*` |

## Setup Methods

### Method 1: Using Scripts (Recommended)

#### PowerShell (Windows)
```powershell
# Run the PowerShell script
.\set-env-vars.ps1

# Then start the application
dotnet run --project CouponHub.Api
```

#### Command Prompt (Windows)
```cmd
# Run the batch script
set-env-vars.bat

# Then start the application
dotnet run --project CouponHub.Api
```

### Method 2: Manual Environment Variables

#### Windows PowerShell
```powershell
$env:DB_CONNECTION_STRING = "Host=localhost;Database=COUPONHUB;Username=postgres;Password=123456"
$env:JWT_KEY = "your_super_secret_jwt_key_here"
$env:JWT_ISSUER = "couponhub"
$env:JWT_AUDIENCE = "couponhub_clients"
$env:JWT_EXPIRY_MINUTES = "120"
$env:LOG_LEVEL = "Information"
$env:ALLOWED_HOSTS = "*"
```

#### Windows Command Prompt
```cmd
set DB_CONNECTION_STRING=Host=localhost;Database=COUPONHUB;Username=postgres;Password=123456
set JWT_KEY=your_super_secret_jwt_key_here
set JWT_ISSUER=couponhub
set JWT_AUDIENCE=couponhub_clients
set JWT_EXPIRY_MINUTES=120
set LOG_LEVEL=Information
set ALLOWED_HOSTS=*
```

#### Linux/macOS
```bash
export DB_CONNECTION_STRING="Host=localhost;Database=COUPONHUB;Username=postgres;Password=123456"
export JWT_KEY="your_super_secret_jwt_key_here"
export JWT_ISSUER="couponhub"
export JWT_AUDIENCE="couponhub_clients"
export JWT_EXPIRY_MINUTES="120"
export LOG_LEVEL="Information"
export ALLOWED_HOSTS="*"
```

### Method 3: Create .env File (Manual)

Create a `.env` file in the project root with:
```
DB_CONNECTION_STRING=Host=localhost;Database=COUPONHUB;Username=postgres;Password=123456
JWT_KEY=your_super_secret_jwt_key_here
JWT_ISSUER=couponhub
JWT_AUDIENCE=couponhub_clients
JWT_EXPIRY_MINUTES=120
LOG_LEVEL=Information
ALLOWED_HOSTS=*
```

## Security Notes

⚠️ **Important Security Considerations:**

1. **Change JWT_KEY**: Use a strong, random secret key in production
2. **Database Credentials**: Use strong passwords and secure connection strings
3. **Environment Files**: Never commit `.env` files to version control
4. **Production**: Use proper secret management (Azure Key Vault, AWS Secrets Manager, etc.)

## Running the Application

After setting environment variables, run:
```bash
dotnet run --project CouponHub.Api
```

The application will read from environment variables and fall back to defaults in `appsettings.json` if not set.
