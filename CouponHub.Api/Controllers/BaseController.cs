using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CouponHub.Business.Extensions;
using System.Security.Claims;

namespace CouponHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleServiceCenterAuthorization(int? requestedServiceCenterId, Func<int?, Task<IActionResult>> authorizedAction)
        {
            ArgumentNullException.ThrowIfNull(authorizedAction);

            var authResult = User.GetEffectiveServiceCenterId(requestedServiceCenterId);
            
            if (!authResult.IsAuthorized)
            {
                return Forbid();
            }

            return authorizedAction(authResult.EffectiveServiceCenterId).Result;
        }

        protected async Task<IActionResult> HandleServiceCenterAuthorizationAsync(int? requestedServiceCenterId, Func<int?, Task<IActionResult>> authorizedAction)
        {
            ArgumentNullException.ThrowIfNull(authorizedAction);

            var authResult = User.GetEffectiveServiceCenterId(requestedServiceCenterId);
            
            if (!authResult.IsAuthorized)
            {
                return Forbid();
            }

            return await authorizedAction(authResult.EffectiveServiceCenterId).ConfigureAwait(false);
        }

        // Helper properties for easy access
        protected string? UserRole => User.GetUserRole();
        protected int? UserServiceCenterId => User.GetUserServiceCenterId();
        protected int UserId => User.GetUserId();
        protected bool IsSuperAdmin => User.IsSuperAdmin();
        protected bool IsAdmin => User.IsAdmin();
        protected bool IsCustomer => User.IsCustomer();
    }
}
