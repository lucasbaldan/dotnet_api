using System.Text.Json;
using dotnet_api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace dotnet_api.Services;

public class PermissionService(RoleManager<GrupoUsuarios> roleManager, UserManager<Usuario> userManager) : AuthorizationHandler<PermissionRequirement>
{
    public readonly RoleManager<GrupoUsuarios> _roleManager = roleManager;
    public readonly UserManager<Usuario> _userManager = userManager;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            context.Fail();
            return;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            context.Fail();
            return;
        }

        var roleName = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (string.IsNullOrEmpty(roleName))
        {
            context.Fail();
            return;
        }

        var grupoUsuario = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (grupoUsuario == null || string.IsNullOrEmpty(grupoUsuario.PermissoesJson))
        {
            context.Fail();
            return;
        }

        var permissoes = JsonSerializer.Deserialize<List<int>>(grupoUsuario.PermissoesJson);
        if (permissoes != null && permissoes.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}

public class PermissionRequirement(int permission) : IAuthorizationRequirement
{
    public int Permission { get; } = permission;
}
