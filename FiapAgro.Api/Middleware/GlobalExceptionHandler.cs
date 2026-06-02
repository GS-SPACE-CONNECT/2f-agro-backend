using System.Diagnostics;
using FiapAgro.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Middleware;

/// <summary>
/// Middleware centralizado de tratamento de exceções (RFC 7807 — Problem Details).
/// Intercepta todas as exceções não tratadas, mapeia para o status HTTP correto
/// e retorna um corpo padronizado sem vazar stack traces para o cliente.
/// Registrado via IExceptionHandler (.NET 8) e ativado com app.UseExceptionHandler().
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NaoEncontradoException  => (StatusCodes.Status404NotFound,            "Recurso não encontrado"),
            ConflitoException       => (StatusCodes.Status409Conflict,             "Conflito de dados"),
            RegraDeNegocioException => (StatusCodes.Status400BadRequest,           "Requisição inválida"),
            DomainException         => (StatusCodes.Status422UnprocessableEntity,  "Regra de negócio violada"),
            _                       => (StatusCodes.Status500InternalServerError,  "Erro interno do servidor"),
        };

        if (status >= 500)
            _logger.LogError(exception, "Exceção não tratada: {Message}", exception.Message);
        else
            _logger.LogWarning("Exceção de domínio [{Type}]: {Message}",
                exception.GetType().Name, exception.Message);

        var pd = new ProblemDetails
        {
            Status = status,
            Title  = title,
            Detail = exception.Message,
        };
        pd.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

        context.Response.StatusCode      = status;
        context.Response.ContentType     = "application/problem+json";
        await context.Response.WriteAsJsonAsync(pd, ct);
        return true;
    }
}
