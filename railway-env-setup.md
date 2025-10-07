# Railway Environment Variables Setup
# Run this script to set up your Railway deployment with proper environment variables

# Database Configuration
# Replace with your actual Railway PostgreSQL connection string
# Format: Host=your-host;Database=your-db;Username=your-user;Password=your-password;Port=your-port

# JWT Configuration
# Generate a secure JWT key for production
# You can use: openssl rand -base64 32

# Email Configuration
# Update with your actual email credentials

# Frontend URL
# Update with your actual frontend domain

echo "Setting up Railway environment variables..."
echo ""
echo "Please set the following environment variables in your Railway dashboard:"
echo ""
echo "1. ConnectionStrings__DefaultConnection"
echo "   Value: Your Railway PostgreSQL connection string"
echo ""
echo "2. Jwt__Key"
echo "   Value: A secure random string (use: openssl rand -base64 32)"
echo ""
echo "3. Jwt__Issuer"
echo "   Value: couponhub"
echo ""
echo "4. Jwt__Audience"
echo "   Value: couponhub_clients"
echo ""
echo "5. Email__SmtpHost"
echo "   Value: smtp.gmail.com"
echo ""
echo "6. Email__SmtpPort"
echo "   Value: 587"
echo ""
echo "7. Email__SmtpUsername"
echo "   Value: your-email@gmail.com"
echo ""
echo "8. Email__SmtpPassword"
echo "   Value: your-app-password"
echo ""
echo "9. Email__FromEmail"
echo "   Value: your-email@gmail.com"
echo ""
echo "10. Email__FromName"
echo "    Value: CouponHub Team"
echo ""
echo "11. Frontend__BaseUrl"
echo "    Value: https://your-frontend-domain.com"
echo ""
echo "12. ASPNETCORE_ENVIRONMENT"
echo "    Value: Production"
echo ""
echo "13. PORT"
echo "    Value: (Railway will set this automatically)"
echo ""
echo "After setting these variables, redeploy your application."
