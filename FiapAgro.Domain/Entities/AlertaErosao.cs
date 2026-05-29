using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

public class AlertaErosao : Alerta
{
    public double InclinacaoSolo { get; set; }

    public AlertaErosao(Guid propriedadeId, double probabilidade, double inclinacaoSolo)
        : base(propriedadeId, probabilidade)
    {
        InclinacaoSolo = inclinacaoSolo;
    }

    public override NivelSeveridade CalcularSeveridade() => InclinacaoSolo switch
    {
        > 45 => NivelSeveridade.Critico,
        > 30 => NivelSeveridade.Alto,
        > 15 => NivelSeveridade.Medio,
        _    => NivelSeveridade.Baixo
    };

    public override string Recomendacao() => CalcularSeveridade() switch
    {
        NivelSeveridade.Critico => $"Inclinação crítica de {InclinacaoSolo:F1}° — implementar terraços e cobertura vegetal urgente.",
        NivelSeveridade.Alto    => $"Inclinação de {InclinacaoSolo:F1}° — aplicar curvas de nível e plantio em contorno.",
        NivelSeveridade.Medio   => $"Inclinação de {InclinacaoSolo:F1}° — monitorar erosão e manter cobertura do solo.",
        _                       => $"Inclinação de {InclinacaoSolo:F1}° sem risco significativo de erosão."
    };
}
