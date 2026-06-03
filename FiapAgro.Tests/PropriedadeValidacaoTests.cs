using FiapAgro.Domain.Entities;
using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Tests;

public class PropriedadeValidacaoTests
{
    private static Propriedade CriarPropriedade(
        string nome = "Fazenda Boa Vista",
        string municipio = "Ribeirão Preto",
        string estado = "SP",
        double area = 100.0,
        Coordenada localizacao = default)
        => new(nome, municipio, estado, area, Guid.NewGuid(), localizacao);

    [Theory]
    [InlineData(100.0, 50.0,  true)]
    [InlineData(100.0, 100.0, true)]
    [InlineData(100.0, 150.0, false)]
    public void Propriedade_TemAreaMinima_RetornaCorreto(double area, double minimo, bool esperado)
    {
        var p = CriarPropriedade(area: area);
        Assert.Equal(esperado, p.TemAreaMinima(minimo));
    }

    [Theory]
    [InlineData("SP", "SP",  true)]
    [InlineData("SP", "sp",  true)]   // case-insensitive
    [InlineData("SP", "RJ",  false)]
    [InlineData("MG", "MG",  true)]
    public void Propriedade_PertenceAoEstado_RetornaCorreto(string estado, string consulta, bool esperado)
    {
        var p = CriarPropriedade(estado: estado);
        Assert.Equal(esperado, p.PertenceAoEstado(consulta));
    }

    [Fact]
    public void Propriedade_SemLocalizacao_TemLocalizacaoRetornaFalse()
    {
        var p = CriarPropriedade(localizacao: Coordenada.Vazia);
        Assert.False(p.TemLocalizacao());
    }

    [Fact]
    public void Propriedade_ComLocalizacao_TemLocalizacaoRetornaTrue()
    {
        var coord = new Coordenada(-21.17, -47.81);
        var p = CriarPropriedade(localizacao: coord);
        Assert.True(p.TemLocalizacao());
    }

    [Fact]
    public void Propriedade_Descricao_ContemNomeEstadoEArea()
    {
        var p = CriarPropriedade("Sítio Verde", "Campinas", "SP", 75.5);
        var desc = p.Descricao();
        Assert.Contains("Sítio Verde", desc);
        Assert.Contains("SP", desc);
        Assert.Contains("75,5", desc);
    }

    [Fact]
    public void Propriedade_Descricao_ComLocalizacao_ContemArrobas()
    {
        var coord = new Coordenada(-21.17, -47.81);
        var p = CriarPropriedade(localizacao: coord);
        Assert.Contains("@", p.Descricao());
    }

    [Fact]
    public void Propriedade_CriadaEm_PreenchidaAutomaticamente()
    {
        var antes = DateTime.UtcNow.AddSeconds(-1);
        var p = CriarPropriedade();
        Assert.True(p.CriadoEm >= antes);
    }

    [Fact]
    public void Propriedade_Id_GeradoAutomaticamente()
    {
        var p = CriarPropriedade();
        Assert.NotEqual(Guid.Empty, p.Id);
    }
}
