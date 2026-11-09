
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SessionRoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _requiredRole;

        public SessionRoleAuthorizeAttribute(string requiredRole)
        {
            _requiredRole = requiredRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string? token = context.HttpContext.Session.GetString("AccessToken");
            string? role = context.HttpContext.Session.GetString("Role");

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new RedirectToActionResult(
                    "Login",
                    "Account",
                    null
                );
                return;
            }

            if (!string.Equals(
                role,
                _requiredRole,
                StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
