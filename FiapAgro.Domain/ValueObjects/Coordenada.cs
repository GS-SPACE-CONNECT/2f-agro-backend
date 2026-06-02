using FiapAgro.Domain.Exceptions;

namespace FiapAgro.Domain.ValueObjects;

/// <summary>
/// Value object que representa as coordenadas geográficas (GPS) de uma propriedade rural.
/// Implementado como <c>readonly struct</c> porque coordenadas são valores imutáveis por natureza:
/// não possuem identidade própria, são comparadas por valor (lat/lng) e têm ciclo de vida
/// vinculado ao objeto que as contém — características que tornam o struct mais eficiente
/// que uma class (sem alocação no heap, sem pressão no GC).
/// </summary>
public readonly struct Coordenada : IEquatable<Coordenada>
{
    /// <summary>Latitude em graus decimais. Intervalo válido: -90 a 90.</summary>
    public double Latitude { get; }

    /// <summary>Longitude em graus decimais. Intervalo válido: -180 a 180.</summary>
    public double Longitude { get; }

    /// <summary>
    /// Inicializa uma coordenada validando os limites geográficos.
    /// </summary>
    /// <param name="latitude">Graus decimais entre -90 e 90.</param>
    /// <param name="longitude">Graus decimais entre -180 e 180.</param>
    /// <exception cref="RegraDeNegocioException">Lançada quando os valores estão fora do intervalo válido.</exception>
    public Coordenada(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new RegraDeNegocioException("Latitude deve estar entre -90 e 90.");
        if (longitude < -180 || longitude > 180)
            throw new RegraDeNegocioException("Longitude deve estar entre -180 e 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    /// <summary>Sentinela para coordenada não informada (0°, 0°).</summary>
    public static readonly Coordenada Vazia = new(0, 0);

    /// <summary>Retorna <c>true</c> quando a coordenada não é o valor padrão (0, 0).</summary>
    public bool IsValida() => Latitude != 0 || Longitude != 0;

    public bool Equals(Coordenada other) =>
        Latitude == other.Latitude && Longitude == other.Longitude;

    public override bool Equals(object? obj) =>
        obj is Coordenada other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);

    public static bool operator ==(Coordenada a, Coordenada b) => a.Equals(b);
    public static bool operator !=(Coordenada a, Coordenada b) => !a.Equals(b);

    public override string ToString() =>
        $"({Latitude:F6}, {Longitude:F6})";
}
