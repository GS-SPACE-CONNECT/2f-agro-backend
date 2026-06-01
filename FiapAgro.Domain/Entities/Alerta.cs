using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

/// <summary>
/// Classe base abstrata para todos os tipos de alerta agroclimático.
/// Aplica herança e polimorfismo — cada herdeiro define sua própria severidade e recomendação.
/// </summary>
public abstract class Alerta
{
    private static int _totalCriados = 0;

    public Guid Id { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public Guid PropriedadeId { get; set; }
    public double Probabilidade { get; set; }

    protected Alerta(Guid propriedadeId, double probabilidade)
    {
        Id = Guid.NewGuid();
        CriadoEm = DateTime.UtcNow;
        PropriedadeId = propriedadeId;
        Probabilidade = Math.Clamp(probabilidade, 0.0, 1.0);
        _totalCriados++;
    }

    /// <summary>Contador estático do total de alertas instanciados na sessão.</summary>
    public static int TotalCriados => _totalCriados;

    /// <summary>Calcula o nível de severidade com base nos dados do alerta. Deve ser sobrescrito por cada tipo.</summary>
    public abstract NivelSeveridade CalcularSeveridade();

    /// <summary>Retorna a recomendação textual adequada ao nível de severidade.</summary>
    public abstract string Recomendacao();

    /// <summary>Formata a data de criação no padrão brasileiro.</summary>
    public static string FormatarData(DateTime data) => data.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    public override string ToString() =>
        $"[{GetType().Name}] {FormatarData(CriadoEm)} — Severidade: {CalcularSeveridade()} — Probabilidade: {Probabilidade:P0}";
}
