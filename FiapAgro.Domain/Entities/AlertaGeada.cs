using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

public class AlertaGeada : Alerta
{
    public double TemperaturaMinima { get; set; }

    private AlertaGeada() : base() {}

    public AlertaGeada(Guid propriedadeId, double probabilidade, double temperaturaMinima)
        : base(propriedadeId, probabilidade)
    {
        TemperaturaMinima = temperaturaMinima;
    }

    public override NivelSeveridade CalcularSeveridade() => TemperaturaMinima switch
    {
        < -3 => NivelSeveridade.Critico,
        < 0  => NivelSeveridade.Alto,
        < 3  => NivelSeveridade.Medio,
        _    => NivelSeveridade.Baixo
    };

    public override string Recomendacao() => CalcularSeveridade() switch
    {
        NivelSeveridade.Critico => $"Geada severa prevista ({TemperaturaMinima:F1}°C) — proteger culturas imediatamente.",
        NivelSeveridade.Alto    => $"Risco alto de geada ({TemperaturaMinima:F1}°C) — cobrir plantas sensíveis.",
        NivelSeveridade.Medio   => $"Temperatura mínima de {TemperaturaMinima:F1}°C — monitorar culturas de inverno.",
        _                       => $"Temperatura de {TemperaturaMinima:F1}°C não representa risco de geada."
    };
}
