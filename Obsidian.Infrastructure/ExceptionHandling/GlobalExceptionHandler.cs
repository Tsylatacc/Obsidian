using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace Obsidian.Infrastructure.ExceptionHandling;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {

        var (statusCode, message) = exception switch
        {
            ForbiddenException =>
                (StatusCodes.Status403Forbidden, exception.Message),

            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, exception.Message),

            ConflictException =>
                (StatusCodes.Status409Conflict, exception.Message),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, exception.Message),

            NotImplementedException =>
                (StatusCodes.Status501NotImplemented, exception.Message),

            ValidationException =>
                (StatusCodes.Status400BadRequest, exception.Message),

            InvalidOperationException =>
                (StatusCodes.Status500InternalServerError,
                $"Invalid operation: {exception.Message}"),

            OperationCanceledException =>
                (StatusCodes.Status408RequestTimeout,
                 "The operation took longer than expected."),

            DbUpdateConcurrencyException =>
                (StatusCodes.Status409Conflict,
                 "The record was modified or removed by another user."),

            DbUpdateException dbEx
                when dbEx.InnerException is PostgresException pgEx =>
                (
                    StatusCodes.Status400BadRequest,
                    GetDatabaseErrorMessage(pgEx)
                ),

            DbUpdateException =>
                (StatusCodes.Status400BadRequest,
                 "The data could not be saved."),

            _ =>
                (StatusCodes.Status500InternalServerError,
                 "An internal error occurred.")
        };

        await Results.Problem(
            statusCode: statusCode,
            title: "Request error",
            detail: message
        ).ExecuteAsync(httpContext);

        return true;
    }

    private static string GetDatabaseErrorMessage(PostgresException exception)
    {
        return exception.SqlState switch
        {
            PostgresErrorCodes.UniqueViolation =>
                "A record with this data already exists.",

            PostgresErrorCodes.ForeignKeyViolation =>
                "The operation could not be completed because related records exist.",

            PostgresErrorCodes.NotNullViolation =>
                "Required fields are missing.",

            PostgresErrorCodes.CheckViolation =>
                "The data entered is invalid.",

            PostgresErrorCodes.RestrictViolation =>
                "This record could not be removed because dependencies exist.",

            _ => "The data could not be saved."
        };
    }
}