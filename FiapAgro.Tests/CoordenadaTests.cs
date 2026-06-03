using FiapAgro.Domain.Exceptions;
using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Tests;

public class CoordenadaTests
{
    [Theory]
    [InlineData(-23.5505, -46.6333)]   // São Paulo
    [InlineData(-15.7942, -47.8822)]   // Brasília
    [InlineData(0.0, 0.0)]             // Origem (sentinela Vazia)
    [InlineData(-90.0, -180.0)]        // Limites mínimos
    [InlineData(90.0, 180.0)]          // Limites máximos
    public void Coordenada_LatLngValidos_CriaComSucesso(double lat, double lng)
    {
        var c = new Coordenada(lat, lng);
        Assert.Equal(lat, c.Latitude);
        Assert.Equal(lng, c.Longitude);
    }

    [Theory]
    [InlineData(91.0, 0.0)]
    [InlineData(-91.0, 0.0)]
    public void Coordenada_LatitudeForaDosLimites_ThrowsRegraDeNegocioException(double lat, double lng)
    {
        Assert.Throws<RegraDeNegocioException>(() => new Coordenada(lat, lng));
    }

    [Theory]
    [InlineData(0.0, 181.0)]
    [InlineData(0.0, -181.0)]
    public void Coordenada_LongitudeForaDosLimites_ThrowsRegraDeNegocioException(double lat, double lng)
    {
        Assert.Throws<RegraDeNegocioException>(() => new Coordenada(lat, lng));
    }

    [Fact]
    public void Coordenada_RegraDeNegocioException_HerdaDeDomainException()
    {
        var ex = Assert.Throws<RegraDeNegocioException>(() => new Coordenada(200, 0));
        Assert.IsAssignableFrom<DomainException>(ex);
    }

    [Fact]
    public void Coordenada_Vazia_IsValidaRetornaFalse()
    {
        Assert.False(Coordenada.Vazia.IsValida());
    }

    [Fact]
    public void Coordenada_ComValoresNaoZero_IsValidaRetornaTrue()
    {
        var c = new Coordenada(-23.5, -46.6);
        Assert.True(c.IsValida());
    }

    [Fact]
    public void Coordenada_IgualdadePorValor_MesmaLatLng()
    {
        var a = new Coordenada(-23.5, -46.6);
        var b = new Coordenada(-23.5, -46.6);
        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Coordenada_IgualdadePorValor_DiferentesLatLng()
    {
        var a = new Coordenada(-23.5, -46.6);
        var b = new Coordenada(-15.0, -47.0);
        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    [Fact]
    public void Coordenada_ToString_ContemLatELng()
    {
        var c = new Coordenada(-23.5505, -46.6333);
        var str = c.ToString();
        Assert.Contains("-23", str);
        Assert.Contains("-46", str);
    }
}
