using FiapAgro.Domain.Enums;
using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Domain.Entities;

/// <summary>
/// Representa um talhão/cultura dentro de uma propriedade rural.
/// Cada lavoura tem uma cultura, área, estado de saúde e localização geográfica.
/// </summary>
public class Lavoura
{
    public Guid Id { get; private set; }
    public Guid PropriedadeId { get; set; }
    public CulturaTipo Cultura { get; set; }
    public string Identificador { get; set; }
    public double AreaHectares { get; set; }
    public SaudeLavoura Saude { get; set; }
    public double? NdviAtual { get; set; }
    public DateTime? UltimaLeitura { get; set; }
    public Coordenada Coordenadas { get; set; }
    public DateTime CriadoEm { get; private set; }

    // Construtor sem parâmetros exigido pelo EF Core para materialização via reflexão.
    private Lavoura() { Identificador = string.Empty; }

    public Lavoura(
        Guid propriedadeId,
        CulturaTipo cultura,
        string identificador,
        double areaHectares,
        SaudeLavoura saude = SaudeLavoura.Saudavel,
        double? ndviAtual = null,
        DateTime? ultimaLeitura = null,
        Coordenada coordenadas = default)
    {
        Id = Guid.NewGuid();
        PropriedadeId = propriedadeId;
        Cultura = cultura;
        Identificador = identificador;
        AreaHectares = areaHectares;
        Saude = saude;
        NdviAtual = ndviAtual;
        UltimaLeitura = ultimaLeitura;
        Coordenadas = coordenadas;
        CriadoEm = DateTime.UtcNow;
    }
}
