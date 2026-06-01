using FiapAgro.Domain.Entities;

namespace FiapAgro.Api.Dtos;

// --- Requests por tipo de alerta ---

public record AlertaPragaRequest(Guid PropriedadeId, double Probabilidade, string EspeciePraga, string CulturaAfetada);
public record AlertaSecaRequest(Guid PropriedadeId, double Probabilidade, int DiasSemChuva);
public record AlertaGeadaRequest(Guid PropriedadeId, double Probabilidade, double TemperaturaMinima);
public record AlertaEnchenteRequest(Guid PropriedadeId, double Probabilidade, double VolumeMM);
public record AlertaErosaoRequest(Guid PropriedadeId, double Probabilidade, double InclinacaoSolo);

// --- Response genérico ---
// Usa polimorfismo: CalcularSeveridade() e Recomendacao() são virtuais e
// retornam valores específicos de cada subtipo em tempo de execução.

public record AlertaResponse(
    Guid Id,
    Guid PropriedadeId,
    string Tipo,
    string Severidade,
    double Probabilidade,
    string Recomendacao,
    string CriadoEm)
{
    public static AlertaResponse FromEntity(Alerta a) => new(
        a.Id,
        a.PropriedadeId,
        a.GetType().Name,
        a.CalcularSeveridade().ToString(),
        a.Probabilidade,
        a.Recomendacao(),
        Alerta.FormatarData(a.CriadoEm));
}
