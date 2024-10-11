using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Image_guesser.SharedKernel;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RequireLoginAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if the user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            // If the user is not authenticated, redirect to the login page
            context.Result = new RedirectToPageResult("/Auth/Login");
        }
    }
}
