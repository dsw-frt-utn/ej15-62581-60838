using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Dsw2026Ej15.Domain.Exceptions;

namespace Dsw2026Ej15.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                // i. Cuando es ValidationException, retornamos 400 Bad Request
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var result = JsonSerializer.Serialize(new { error = ex.Message });
                await context.Response.WriteAsync(result);
            }
            catch (Exception ex)
            {
                // ii. En cualquier otro caso, retornamos 500
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var result = JsonSerializer.Serialize(new
                {
                    status = 500,
                    title = "Ocurrió un error inesperado al procesar la solicitud.",
                    detail = ex.Message
                });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
