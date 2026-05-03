using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CafeManagement.Filters
{
    // FIX: params string[] roles clearly handles multi-role case
    public class SessionAuthorizeAttribute : ActionFilterAttribute {
        private readonly string[] _roles;

        public SessionAuthorizeAttribute(params string[] roles) {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            var session = context.HttpContext.Session;
            var userId = session.GetString("UserId");

            // Not logged in
            if (string.IsNullOrEmpty(userId)) {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            // Role check — FIX: if no roles specified, any logged-in user passes
            if (_roles.Length > 0) {
                var userRole = session.GetString("UserRole") ?? "";
                bool hasRole = _roles.Any(r =>
                    string.Equals(r, userRole, StringComparison.OrdinalIgnoreCase));
                if (!hasRole) {
                    context.Result = new RedirectToActionResult("Login", "Auth",
                        new { error = "Access denied." });
                    return;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
