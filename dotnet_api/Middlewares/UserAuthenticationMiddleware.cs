using dotnet_api.Models;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace dotnet_api.Middlewares;

public class UserAuthtenticationMiddleware(RequestDelegate next, IServiceScopeFactory serviceScope)
{
    private readonly RequestDelegate _next = next;
    private readonly IServiceScopeFactory serviceScope = serviceScope;

    public async Task Invoke(HttpContext context)
    {
        if (context.User.Identity!.IsAuthenticated)
        {

            using var scope = serviceScope.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Usuário não autenticado.",
                    Errors = ["Usuário não autenticado"],
                    StackTrace = "API - Middleware Authetication"
                }));
            }

            var user = await userManager.FindByIdAsync(userId);

            if (user == null || user.LockoutEnabled || user.RefreshToken == null)
            {
                context.Response.StatusCode = (int)StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = JsonSerializer.Serialize(new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Message = "Usuário bloqueado ou revogado no sistema!",
                    Errors = ["Usuário bloqueado ou revogado no sistema!"],
                    StackTrace = "API - Middleware Authetication"
                });

                await context.Response.WriteAsync(response);
                return;
            }
        }

        await _next(context);
    }
}