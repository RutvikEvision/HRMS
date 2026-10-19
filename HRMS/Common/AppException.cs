using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HRMS.Common
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public AppException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            if (exception is FluentValidation.ValidationException validation)
            {
                var errors = validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(
                    new ValidationProblemDetails(errors) { Status = 400 }, ct);
                return true;
            }

            if (exception is not AppException ex) return false;

            context.Response.StatusCode = ex.StatusCode;
            await context.Response.WriteAsJsonAsync(
                new ProblemDetails { Status = ex.StatusCode, Detail = ex.Message }, ct);
            return true;
        }
    }

}
