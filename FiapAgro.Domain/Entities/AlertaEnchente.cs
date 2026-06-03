using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

public class AlertaEnchente : Alerta
{
    public double VolumeMM { get; set; }

    private AlertaEnchente() : base() {}

    public AlertaEnchente(Guid propriedadeId, double probabilidade, double volumeMM)
        : base(propriedadeId, probabilidade)
    {
        VolumeMM = volumeMM;
    }

    public override NivelSeveridade CalcularSeveridade() => VolumeMM switch
    {
        > 100 => NivelSeveridade.Critico,
        > 50  => NivelSeveridade.Alto,
        > 20  => NivelSeveridade.Medio,
        _     => NivelSeveridade.Baixo
    };

    public override string Recomendacao() => CalcularSeveridade() switch
    {
        NivelSeveridade.Critico => $"Volume crítico de {VolumeMM:F0}mm — evacuar áreas baixas e acionar defesa civil.",
        NivelSeveridade.Alto    => $"Precipitação intensa de {VolumeMM:F0}mm — verificar drenos e valetas.",
        NivelSeveridade.Medio   => $"Chuva moderada de {VolumeMM:F0}mm — monitorar nível de córregos.",
        _                       => $"Precipitação de {VolumeMM:F0}mm dentro da normalidade."
    };
}
