using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;
using FiapAgro.Domain.Helpers;
using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Tests;

public class LavouraTests
{
    [Fact]
    public void Construtor_DeveGerarIdECriadoEm()
    {
        var lavoura = new Lavoura(Guid.NewGuid(), CulturaTipo.Milho, "L1", 50.0);

        Assert.NotEqual(Guid.Empty, lavoura.Id);
        Assert.True(lavoura.CriadoEm <= DateTime.UtcNow);
    }

    [Fact]
    public void Construtor_SaudeDefaultDeveSerSaudavel()
    {
        var lavoura = new Lavoura(Guid.NewGuid(), CulturaTipo.Soja, "L2", 100.0);

        Assert.Equal(SaudeLavoura.Saudavel, lavoura.Saude);
    }

    [Fact]
    public void Construtor_NdviDeveSerNullPorPadrao()
    {
        var lavoura = new Lavoura(Guid.NewGuid(), CulturaTipo.Feijao, "L1", 30.0);

        Assert.Null(lavoura.NdviAtual);
        Assert.Null(lavoura.UltimaLeitura);
    }

    [Fact]
    public void Construtor_DevePersistirCoordenadas()
    {
        var coord = new Coordenada(-21.0, -47.0);
        var lavoura = new Lavoura(Guid.NewGuid(), CulturaTipo.Cana, "L3", 200.0,
            coordenadas: coord);

        Assert.Equal(-21.0, lavoura.Coordenadas.Latitude);
        Assert.Equal(-47.0, lavoura.Coordenadas.Longitude);
    }

    [Theory]
    [InlineData("milho", CulturaTipo.Milho)]
    [InlineData("tomate", CulturaTipo.Tomate)]
    [InlineData("feijao", CulturaTipo.Feijao)]
    [InlineData("cana", CulturaTipo.Cana)]
    [InlineData("soja", CulturaTipo.Soja)]
    [InlineData("mandioca", CulturaTipo.Mandioca)]
    [InlineData("alface", CulturaTipo.Alface)]
    public void EnumHelper_FromSnakeCase_DeveConverterCultura(string snakeCase, CulturaTipo esperado)
    {
        var resultado = EnumHelper.FromSnakeCase<CulturaTipo>(snakeCase);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData(CulturaTipo.Milho, "milho")]
    [InlineData(CulturaTipo.Feijao, "feijao")]
    [InlineData(CulturaTipo.Cana, "cana")]
    public void EnumHelper_ToSnakeCase_DeveConverterCultura(CulturaTipo cultura, string esperado)
    {
        var resultado = EnumHelper.ToSnakeCase(cultura.ToString());
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData("saudavel", SaudeLavoura.Saudavel)]
    [InlineData("atencao", SaudeLavoura.Atencao)]
    [InlineData("risco", SaudeLavoura.Risco)]
    [InlineData("perdida", SaudeLavoura.Perdida)]
    public void EnumHelper_FromSnakeCase_DeveConverterSaude(string snakeCase, SaudeLavoura esperado)
    {
        var resultado = EnumHelper.FromSnakeCase<SaudeLavoura>(snakeCase);
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void EnumHelper_CulturaLabel_DeveRetornarLabelPtBr()
    {
        Assert.Equal("Feijão", EnumHelper.CulturaLabel(CulturaTipo.Feijao));
        Assert.Equal("Cana-de-açúcar", EnumHelper.CulturaLabel(CulturaTipo.Cana));
        Assert.Equal("Milho", EnumHelper.CulturaLabel(CulturaTipo.Milho));
    }
}
