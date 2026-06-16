using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Pizza_API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Log unexpected errors (not validation/not found — those are expected)
            if (exception is not NotFoundException && exception is not ValidationException)
                _logger.LogError(exception, "Unhandled exception occurred");

            var (status, title) = exception switch
            {
                NotFoundException ex => (404, ex.Message),
                ValidationException ex => (400, ex.Message),
                UnauthorizedAccessException => (401, "Unauthorized"),
                ConflictException ex => (409, ex.Message),
                _ => (500, "An unexpected error occurred")
            };

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title
            };

            httpContext.Response.StatusCode = status;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}