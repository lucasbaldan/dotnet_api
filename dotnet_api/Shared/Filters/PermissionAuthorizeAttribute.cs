using dotnet_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_api.Shared.Filters;

[AttributeUsage(AttributeTargets.All)]
public class PermissionAuthorizeAttribute(int permission) : Attribute, IAuthorizationFilter
{

    private readonly int _permission = (int)permission;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
        var user = context.HttpContext.User;

        var permissionRequirement = new PermissionRequirement(_permission);
        var authResult = authService.AuthorizeAsync(user, null, permissionRequirement).Result;

        if (!authResult.Succeeded)
        {
            context.Result = new ForbidResult();
        }
    }
}
