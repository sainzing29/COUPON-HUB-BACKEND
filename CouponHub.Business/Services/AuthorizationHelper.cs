using System.Security.Claims;

namespace CouponHub.Business.Services
{
    public class AuthorizationHelper
    {
        public class AuthorizationResult
        {
            public bool IsAuthorized { get; set; }
            public int? EffectiveServiceCenterId { get; set; }
            public string? ErrorMessage { get; set; }
        }

        public static AuthorizationResult GetEffectiveServiceCenterId(ClaimsPrincipal user, int? requestedServiceCenterId)
        {
            var role = user.FindFirst(ClaimTypes.Role)?.Value;
            var scClaim = user.FindFirst("serviceCenterId")?.Value;
            
            int? scClaimId = null;
            if (int.TryParse(scClaim, out var parsed))
            {
                scClaimId = parsed;
            }

            return role switch
            {
                "SuperAdmin" => new AuthorizationResult
                {
                    IsAuthorized = true,
                    EffectiveServiceCenterId = requestedServiceCenterId, // SuperAdmin can request any service center
                    ErrorMessage = null
                },
                "Admin" => new AuthorizationResult
                {
                    IsAuthorized = scClaimId.HasValue,
                    EffectiveServiceCenterId = scClaimId, // Admin can only see their own service center
                    ErrorMessage = scClaimId.HasValue ? null : "Admin user must have a service center assigned"
                },
                _ => new AuthorizationResult
                {
                    IsAuthorized = false,
                    EffectiveServiceCenterId = null,
                    ErrorMessage = "Access denied. Insufficient permissions."
                }
            };
        }

        public static string? GetUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        public static int? GetUserServiceCenterId(ClaimsPrincipal user)
        {
            var scClaim = user.FindFirst("serviceCenterId")?.Value;
            return int.TryParse(scClaim, out var parsed) ? parsed : null;
        }

        public static int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out var userId) ? userId : 0;
        }
    }
}
