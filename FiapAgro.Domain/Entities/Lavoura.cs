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

    /// <summary>
    /// Registra uma nova leitura de NDVI vinda do satélite/sensor,
    /// atualizando o índice e o timestamp da última leitura.
    /// </summary>
    public void RegistrarLeitura(double ndvi)
    {
        NdviAtual = Math.Clamp(ndvi, -1.0, 1.0);
        UltimaLeitura = DateTime.UtcNow;
    }

    /// <summary>
    /// Calcula quantos dias se passaram desde a última leitura de sensor.
    /// Retorna <c>null</c> se nenhuma leitura foi registrada.
    /// </summary>
    public int? DiasDesdeUltimaLeitura() =>
        UltimaLeitura.HasValue
            ? (int)(DateTime.UtcNow - UltimaLeitura.Value).TotalDays
            : null;

    /// <summary>
    /// Indica se a lavoura está com dados desatualizados (sem leitura há mais de N dias).
    /// </summary>
    public bool LeituraDesatualizada(int diasLimite = 7) =>
        DiasDesdeUltimaLeitura() is { } dias && dias > diasLimite;
}
