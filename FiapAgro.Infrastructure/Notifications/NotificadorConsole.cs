using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Notifications;

public class NotificadorConsole : INotificador
{
    private readonly ILogger<NotificadorConsole> _logger;

    public NotificadorConsole(ILogger<NotificadorConsole> logger)
    {
        _logger = logger;
    }

    public Task NotificarAsync(Alerta alerta)
    {
        _logger.LogWarning(
            "[ALERTA] {Tipo} — Severidade: {Severidade} — {Recomendacao}",
            alerta.GetType().Name,
            alerta.CalcularSeveridade(),
            alerta.Recomendacao());
        return Task.CompletedTask;
    }

    public Task NotificarLoteAsync(IEnumerable<Alerta> alertas)
    {
        foreach (var alerta in alertas)
            NotificarAsync(alerta).GetAwaiter().GetResult();
        return Task.CompletedTask;
    }
}
