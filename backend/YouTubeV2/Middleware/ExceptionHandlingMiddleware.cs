using FluentValidation;
using YouTubeV2.Application.DTO;
using YouTubeV2.Application.Exceptions;

namespace YouTubeV2.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException validationException)
            {
                await HandleValidationExceptionAsync(httpContext, validationException);
            }
            catch (BadRequestException badRequestException)
            {
                await HandleBadRequestException(httpContext, badRequestException);
            }
            catch (ArgumentException argumentException)
            {
                await HandleArgumentExceptionAsync(httpContext, argumentException);
            }
            catch (ForbiddenException forbiddenException)
            {
                await HandleForbiddenExcaption(httpContext, forbiddenException);
            }
            catch (UnauthorizedException unauthorizedException)
            {
                await HandleUnauthorizedExceptionAsync(httpContext, unauthorizedException);
            }
            catch (Exception notFoundException) when (notFoundException is FileNotFoundException || notFoundException is NotFoundException)
            {
                await HandleNotFoundExceptionAsync(httpContext, notFoundException);
            }
            catch (Exception exception)
            {
                HandleException(httpContext, exception);
            }
        }

        private static async Task HandleValidationExceptionAsync(HttpContext httpContext, ValidationException validationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var mappedErrors = validationException.Errors.Select(error => new ErrorResponseDTO(error.ErrorMessage));
            await httpContext.Response.WriteAsJsonAsync(mappedErrors);
        }

        private static async Task HandleBadRequestException(HttpContext httpContext, BadRequestException badRequestException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var mappedErrors = badRequestException.Errors;
            await httpContext.Response.WriteAsJsonAsync(mappedErrors);
        }

        private static async Task HandleArgumentExceptionAsync(HttpContext httpContext, ArgumentException argumentException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponseDTO(argumentException.Message));
        }

        private static async Task HandleForbiddenExcaption(HttpContext httpContext, ForbiddenException forbiddenException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponseDTO(forbiddenException.Message));
        }

        private static async Task HandleNotFoundExceptionAsync(HttpContext httpContext, Exception notFoundException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponseDTO(notFoundException.Message));
        }

        private static async Task HandleUnauthorizedExceptionAsync(HttpContext httpContext, UnauthorizedException unauthorizedException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponseDTO(unauthorizedException.Message));
        }

        private void HandleException(HttpContext httpContext, Exception exception)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            if (!_webHostEnvironment.IsProduction())
            {
                _logger.LogInformation(
                "Request {method} {url} => {statusCode}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                httpContext.Response.StatusCode);
                _logger.LogInformation("Exception message: {ExceptionMessage}", exception.Message);
                _logger.LogInformation("Exception source: {ExceptionSource}", exception.Source);
                _logger.LogInformation("Exception stack trace: {ExceptionStackTrace}", exception.StackTrace);
            }
        }
    }
}

    