using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;

namespace FiapAgro.Tests;

public class AlertaSeveridadeTests
{
    private static readonly Guid _propId = Guid.NewGuid();

    // ── AlertaPraga ─────────────────────────────────────────────────────────
    [Theory]
    [InlineData(0.90, NivelSeveridade.Critico)]
    [InlineData(0.85, NivelSeveridade.Critico)]
    [InlineData(0.70, NivelSeveridade.Alto)]
    [InlineData(0.65, NivelSeveridade.Alto)]
    [InlineData(0.50, NivelSeveridade.Medio)]
    [InlineData(0.40, NivelSeveridade.Medio)]
    [InlineData(0.20, NivelSeveridade.Baixo)]
    public void AlertaPraga_CalcularSeveridade_RetornaNivelCorreto(double prob, NivelSeveridade esperado)
    {
        var alerta = new AlertaPraga(_propId, prob, "lagarta-do-cartucho", "milho");
        Assert.Equal(esperado, alerta.CalcularSeveridade());
    }

    [Fact]
    public void AlertaPraga_Recomendacao_NaoCritica_NaoEstaVazia()
    {
        var alerta = new AlertaPraga(_propId, 0.20, "pulgão", "trigo");
        Assert.NotEmpty(alerta.Recomendacao());
    }

    // ── AlertaSeca ──────────────────────────────────────────────────────────
    [Theory]
    [InlineData(25, NivelSeveridade.Critico)]
    [InlineData(22, NivelSeveridade.Critico)]
    [InlineData(15, NivelSeveridade.Alto)]
    [InlineData(10, NivelSeveridade.Medio)]
    [InlineData(5,  NivelSeveridade.Baixo)]
    public void AlertaSeca_CalcularSeveridade_RetornaNivelCorreto(int dias, NivelSeveridade esperado)
    {
        var alerta = new AlertaSeca(_propId, 0.5, dias);
        Assert.Equal(esperado, alerta.CalcularSeveridade());
    }

    // ── AlertaGeada ─────────────────────────────────────────────────────────
    [Theory]
    [InlineData(-5.0, NivelSeveridade.Critico)]
    [InlineData(-1.0, NivelSeveridade.Alto)]
    [InlineData(1.5,  NivelSeveridade.Medio)]
    [InlineData(5.0,  NivelSeveridade.Baixo)]
    public void AlertaGeada_CalcularSeveridade_RetornaNivelCorreto(double temp, NivelSeveridade esperado)
    {
        var alerta = new AlertaGeada(_propId, 0.6, temp);
        Assert.Equal(esperado, alerta.CalcularSeveridade());
    }

    // ── AlertaEnchente ──────────────────────────────────────────────────────
    [Theory]
    [InlineData(120.0, NivelSeveridade.Critico)]
    [InlineData(60.0,  NivelSeveridade.Alto)]
    [InlineData(30.0,  NivelSeveridade.Medio)]
    [InlineData(10.0,  NivelSeveridade.Baixo)]
    public void AlertaEnchente_CalcularSeveridade_RetornaNivelCorreto(double mm, NivelSeveridade esperado)
    {
        var alerta = new AlertaEnchente(_propId, 0.7, mm);
        Assert.Equal(esperado, alerta.CalcularSeveridade());
    }

    // ── AlertaErosao ────────────────────────────────────────────────────────
    [Theory]
    [InlineData(50.0, NivelSeveridade.Critico)]
    [InlineData(35.0, NivelSeveridade.Alto)]
    [InlineData(20.0, NivelSeveridade.Medio)]
    [InlineData(10.0, NivelSeveridade.Baixo)]
    public void AlertaErosao_CalcularSeveridade_RetornaNivelCorreto(double graus, NivelSeveridade esperado)
    {
        var alerta = new AlertaErosao(_propId, 0.4, graus);
        Assert.Equal(esperado, alerta.CalcularSeveridade());
    }

    // ── Contador estático ───────────────────────────────────────────────────
    [Fact]
    public void Alerta_TotalCriados_IncrementaACadaInstancia()
    {
        var antes = Alerta.TotalCriados;
        _ = new AlertaPraga(_propId, 0.5, "praga", "cultura");
        _ = new AlertaSeca(_propId, 0.5, 10);
        Assert.Equal(antes + 2, Alerta.TotalCriados);
    }

    // ── ToString / FormatarData ─────────────────────────────────────────────
    [Fact]
    public void Alerta_ToString_ContemTipoESeveridade()
    {
        var alerta = new AlertaPraga(_propId, 0.90, "broca", "cana");
        var str = alerta.ToString();
        Assert.Contains("AlertaPraga", str);
        Assert.Contains("Critico", str);
    }

    [Fact]
    public void Alerta_ProbabilidadeClampada_EntreZeroEUm()
    {
        var acima = new AlertaSeca(_propId, 1.5, 5);
        var abaixo = new AlertaSeca(_propId, -0.5, 5);
        Assert.Equal(1.0, acima.Probabilidade);
        Assert.Equal(0.0, abaixo.Probabilidade);
    }
}
