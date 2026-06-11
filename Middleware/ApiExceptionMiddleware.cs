using System.Net;
using PJATK_APBD_Hospital.Contracts.Common;
using PJATK_APBD_Hospital.Exceptions;

namespace PJATK_APBD_Hospital.Middleware;

public sealed class ApiExceptionMiddleware(
    RequestDelegate next,
    ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundApiException exception)
        {
            await WriteErrorAsync(
                context,
                HttpStatusCode.NotFound,
                "NotFound",
                exception.Message);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing the request.");

            await WriteErrorAsync(
                context,
                HttpStatusCode.InternalServerError,
                "InternalServerError",
                "Unexpected server error.");
        }
    }

    private static async Task WriteErrorAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string error,
        string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse((int)statusCode, error, message);
        await context.Response.WriteAsJsonAsync(response);
    }
}
