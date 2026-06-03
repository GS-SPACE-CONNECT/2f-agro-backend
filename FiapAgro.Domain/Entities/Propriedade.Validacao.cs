namespace FiapAgro.Domain.Entities;

/// <summary>
/// Parte parcial de <see cref="Propriedade"/> dedicada aos métodos de consulta e validação.
/// Separada do arquivo principal para manter o arquivo de estado enxuto e agrupar
/// a lógica de comportamento em um único lugar de fácil localização.
/// </summary>
public partial class Propriedade
{
    /// <summary>Verifica se a propriedade atinge a área mínima exigida.</summary>
    public bool TemAreaMinima(double minHectares) => AreaHectares >= minHectares;

    /// <summary>Verifica se a propriedade pertence ao estado informado (case-insensitive).</summary>
    public bool PertenceAoEstado(string estado) =>
        string.Equals(Estado, estado, StringComparison.OrdinalIgnoreCase);

    /// <summary>Indica se a propriedade possui coordenadas GPS registradas.</summary>
    public bool TemLocalizacao() => Localizacao.IsValida();

    /// <summary>Retorna uma descrição legível da propriedade com localização quando disponível.</summary>
    public string Descricao() =>
        $"{Nome} — {Municipio}/{Estado} ({AreaHectares:F1} ha){(TemLocalizacao() ? $" @ {Localizacao}" : string.Empty)}";
}
