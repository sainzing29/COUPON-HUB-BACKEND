using System.Security.Claims;
using CouponHub.Business.Services;

namespace CouponHub.Business.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static AuthorizationHelper.AuthorizationResult GetEffectiveServiceCenterId(this ClaimsPrincipal user, int? requestedServiceCenterId)
        {
            return AuthorizationHelper.GetEffectiveServiceCenterId(user, requestedServiceCenterId);
        }

        public static string? GetUserRole(this ClaimsPrincipal user)
        {
            return AuthorizationHelper.GetUserRole(user);
        }

        public static int? GetUserServiceCenterId(this ClaimsPrincipal user)
        {
            return AuthorizationHelper.GetUserServiceCenterId(user);
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return AuthorizationHelper.GetUserId(user);
        }

        public static bool IsSuperAdmin(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == "SuperAdmin";
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == "Admin";
        }

        public static bool IsCustomer(this ClaimsPrincipal user)
        {
            return user.GetUserRole() == "Customer";
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst("email")?.Value;
        }

        public static string? GetUserMobileNumber(this ClaimsPrincipal user)
        {
            return user.FindFirst("mobileNumber")?.Value;
        }

        public static string? GetUserLastName(this ClaimsPrincipal user)
        {
            return user.FindFirst("lastName")?.Value;
        }

        public static string GetUserFullName(this ClaimsPrincipal user)
        {
            var firstName = user.FindFirst(ClaimTypes.Name)?.Value ?? "";
            var lastName = user.FindFirst("lastName")?.Value ?? "";
            return $"{firstName} {lastName}".Trim();
        }

        public static bool GetUserIsActive(this ClaimsPrincipal user)
        {
            if (bool.TryParse(user.FindFirst("isActive")?.Value, out var isActive))
            {
                return isActive;
            }
            return true; // Default to active if not found
        }
    }
}
