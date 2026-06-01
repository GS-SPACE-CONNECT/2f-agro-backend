using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

public class AlertaPraga : Alerta
{
    public string EspeciePraga { get; set; }
    public string CulturaAfetada { get; set; }

    public AlertaPraga(Guid propriedadeId, double probabilidade, string especiePraga, string culturaAfetada)
        : base(propriedadeId, probabilidade)
    {
        EspeciePraga = especiePraga;
        CulturaAfetada = culturaAfetada;
    }

    public override NivelSeveridade CalcularSeveridade() => Probabilidade switch
    {
        >= 0.85 => NivelSeveridade.Critico,
        >= 0.65 => NivelSeveridade.Alto,
        >= 0.40 => NivelSeveridade.Medio,
        _ => NivelSeveridade.Baixo
    };

    public override string Recomendacao() => CalcularSeveridade() switch
    {
        NivelSeveridade.Critico => $"Aplicar defensivo imediatamente contra {EspeciePraga} na cultura de {CulturaAfetada}.",
        NivelSeveridade.Alto    => $"Monitorar lavoura de {CulturaAfetada} e preparar defensivo contra {EspeciePraga}.",
        NivelSeveridade.Medio   => $"Inspecionar plantas de {CulturaAfetada} para sinais de {EspeciePraga}.",
        _                       => $"Observar evolução da praga {EspeciePraga} nos próximos dias."
    };
}
