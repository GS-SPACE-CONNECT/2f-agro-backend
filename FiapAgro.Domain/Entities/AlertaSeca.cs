using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

public class AlertaSeca : Alerta
{
    public int DiasSemChuva { get; set; }

    private AlertaSeca() : base() {}

    public AlertaSeca(Guid propriedadeId, double probabilidade, int diasSemChuva)
        : base(propriedadeId, probabilidade)
    {
        DiasSemChuva = diasSemChuva;
    }

    public override NivelSeveridade CalcularSeveridade() => DiasSemChuva switch
    {
        > 21 => NivelSeveridade.Critico,
        > 14 => NivelSeveridade.Alto,
        > 7  => NivelSeveridade.Medio,
        _    => NivelSeveridade.Baixo
    };

    public override string Recomendacao() => CalcularSeveridade() switch
    {
        NivelSeveridade.Critico => $"Ativar irrigação de emergência — {DiasSemChuva} dias sem chuva.",
        NivelSeveridade.Alto    => $"Aumentar frequência de irrigação — {DiasSemChuva} dias sem chuva.",
        NivelSeveridade.Medio   => $"Monitorar umidade do solo — {DiasSemChuva} dias sem chuva.",
        _                       => "Condições dentro do esperado para o período."
    };
}
