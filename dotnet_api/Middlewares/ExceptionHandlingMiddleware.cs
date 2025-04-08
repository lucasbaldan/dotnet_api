using dotnet_api.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
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

                var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = contextFeature?.Error;
                var statusCode = (int)HttpStatusCode.InternalServerError;
                
                statusCode = exception switch
                {
                    ArgumentException => (int)HttpStatusCode.BadRequest,
                    FormatException => (int)HttpStatusCode.BadRequest,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.InternalServerError,
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                var response = JsonSerializer.Serialize(
                    new ErrorResponse
                    {
                        StatusCode = statusCode,
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
