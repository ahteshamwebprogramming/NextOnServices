using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextOnServices.Infrastructure.BL
{
    public class LoginSourceMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginSourceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the user is authenticated
            if (context.User.Identity.IsAuthenticated)
            {
                var loginSourceClaim = context.User.Claims.FirstOrDefault(c => c.Type == "LoginSource");
                if (loginSourceClaim != null && loginSourceClaim.Value == "SupplierLogin")
                {
                    // If loginSource is SupplierLogin, proceed with the specific logic
                    // For example, redirect to a Supplier-specific page or action
                    context.Response.Redirect("/VT/Supplier/Dashboard");
                    return;
                }
            }

            // Continue to the next middleware in the pipeline
            await _next(context);
        }
    }
}
