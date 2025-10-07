# Railway Environment Variables Setup Guide

## Required Environment Variables

Set these in your Railway dashboard under your service's "Variables" tab:

### 1. Database Configuration
```
DATABASE_URL=postgresql://username:password@host:port/database
```
OR
```
ConnectionStrings__DefaultConnection=Host=your-host;Database=your-db;Username=your-user;Password=your-password;Port=your-port
```

### 2. JWT Configuration
```
JWT_KEY=your-secure-random-key-here
Jwt__Key=your-secure-random-key-here
Jwt__Issuer=couponhub
Jwt__Audience=couponhub_clients
```

### 3. Environment
```
ASPNETCORE_ENVIRONMENT=Production
```

### 4. Email Configuration (Optional)
```
Email__SmtpHost=smtp.gmail.com
Email__SmtpPort=587
Email__SmtpUsername=your-email@gmail.com
Email__SmtpPassword=your-app-password
Email__FromEmail=your-email@gmail.com
Email__FromName=CouponHub Team
```

### 5. Frontend URL (Optional)
```
Frontend__BaseUrl=https://your-frontend-domain.com
```

## How to Get Railway PostgreSQL Connection String

1. Go to your Railway dashboard
2. Click on your PostgreSQL service
3. Go to the "Connect" tab
4. Copy the "Connection URL" 
5. Set it as `DATABASE_URL` environment variable

## Generate Secure JWT Key

Run this command to generate a secure JWT key:
```bash
openssl rand -base64 32
```

## Testing Your Deployment

After setting environment variables and redeploying:

1. Check Railway logs for startup messages
2. Test these endpoints:
   - `https://your-domain.up.railway.app/` - Root endpoint
   - `https://your-domain.up.railway.app/health` - Health check
   - `https://your-domain.up.railway.app/swagger` - API documentation

## Common Issues

- **Healthcheck fails**: Usually means missing `DATABASE_URL` or `JWT_KEY`
- **Database connection fails**: Check your PostgreSQL service is running
- **JWT errors**: Ensure `JWT_KEY` is set and is a secure random string
