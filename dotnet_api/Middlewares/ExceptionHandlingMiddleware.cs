using dotnet_api.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace dotnet_api.Middlewares;

public static class ExceptionHandlingMiddleware
{
    public static void UseExceptionHandlingMiddleware(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var response = JsonSerializer.Serialize(
                    new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Message = "Ocorreu um erro ao processar a ação desejada. Contate o Administrador do Sistema",
                        Errors = [contextFeature!.Error.Message],
                        StackTrace = contextFeature!.Error.StackTrace

                    }
                    );

                await context.Response.WriteAsync(response);
            });
        });
    }
}
