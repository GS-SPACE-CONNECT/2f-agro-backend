using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FiapAgro.Infrastructure.Detectors;

/// <summary>
/// Classe abstrata base para detectores de alertas agroclimáticos.
/// Implementa o fluxo genérico de análise e delega a criação do alerta para cada herdeiro.
/// </summary>
public abstract class DetectorBase<T> : IDetector<T> where T : Alerta
{
    protected readonly ILogger _logger;

    protected DetectorBase(ILogger logger)
    {
        _logger = logger;
    }

    public Task<T?> AnalisarAsync(Guid propriedadeId, IDictionary<string, object> dados)
    {
        if (!PodeAnalisar(dados))
        {
            _logger.LogWarning("Dados insuficientes para análise de {Tipo}.", typeof(T).Name);
            return Task.FromResult<T?>(null);
        }

        try
        {
            var alerta = CriarAlerta(propriedadeId, dados);
            _logger.LogInformation(
                "Alerta {Tipo} criado — propriedade {Id} — severidade {Severidade}.",
                typeof(T).Name, propriedadeId, alerta.CalcularSeveridade());
            return Task.FromResult<T?>(alerta);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao analisar dados para {Tipo}.", typeof(T).Name);
            return Task.FromResult<T?>(null);
        }
    }

    public abstract bool PodeAnalisar(IDictionary<string, object> dados);

    protected abstract T CriarAlerta(Guid propriedadeId, IDictionary<string, object> dados);
}
