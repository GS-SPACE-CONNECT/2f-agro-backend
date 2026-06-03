using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;
using FiapAgro.Domain.Exceptions;
using FiapAgro.Domain.Helpers;

namespace FiapAgro.Tests;

public class DiagnosticoTests
{
    [Fact]
    public void Construtor_DeveGerarIdECriadoEm()
    {
        var diag = new DiagnosticoPraga(
            "/foto.jpg", PragaTipo.Sadia, 0.9, NivelSeveridade.Baixo,
            "Saudável.", "+5511999990000");

        Assert.NotEqual(Guid.Empty, diag.Id);
        Assert.True(diag.CriadoEm <= DateTime.UtcNow);
    }

    [Fact]
    public void Construtor_ConfiancaDeveSerClampedEntre0e1()
    {
        var diag1 = new DiagnosticoPraga(
            "/foto.jpg", PragaTipo.Sadia, 1.5, NivelSeveridade.Baixo,
            "Teste.", "+5511999990000");
        Assert.Equal(1.0, diag1.Confianca);

        var diag2 = new DiagnosticoPraga(
            "/foto.jpg", PragaTipo.Sadia, -0.3, NivelSeveridade.Baixo,
            "Teste.", "+5511999990000");
        Assert.Equal(0.0, diag2.Confianca);
    }

    [Fact]
    public void Construtor_LavouraIdDeveSerNullPorPadrao()
    {
        var diag = new DiagnosticoPraga(
            "/foto.jpg", PragaTipo.Oidio, 0.7, NivelSeveridade.Medio,
            "Aplicar fungicida.", "+5511999990000");

        Assert.Null(diag.LavouraId);
    }

    [Fact]
    public void Construtor_LavouraIdDevePersistirQuandoInformado()
    {
        var lavouraId = Guid.NewGuid();
        var diag = new DiagnosticoPraga(
            "/foto.jpg", PragaTipo.Antracnose, 0.85, NivelSeveridade.Alto,
            "Tratar com fungicida.", "+5511999990000",
            lavouraId: lavouraId);

        Assert.Equal(lavouraId, diag.LavouraId);
    }

    [Theory]
    [InlineData("ferrugem_asiatica", PragaTipo.FerrugemAsiatica)]
    [InlineData("lagarta_do_cartucho", PragaTipo.LagartaDoCartucho)]
    [InlineData("mancha_foliar", PragaTipo.ManchaFoliar)]
    [InlineData("oidio", PragaTipo.Oidio)]
    [InlineData("mosca_branca", PragaTipo.MoscaBranca)]
    [InlineData("broca_do_cafe", PragaTipo.BrocaDoCafe)]
    [InlineData("antracnose", PragaTipo.Antracnose)]
    [InlineData("sadia", PragaTipo.Sadia)]
    public void EnumHelper_FromSnakeCase_DeveConverterPragaTipo(string snakeCase, PragaTipo esperado)
    {
        var resultado = EnumHelper.FromSnakeCase<PragaTipo>(snakeCase);
        Assert.Equal(esperado, resultado);
    }

    [Theory]
    [InlineData(PragaTipo.FerrugemAsiatica, "ferrugem_asiatica")]
    [InlineData(PragaTipo.LagartaDoCartucho, "lagarta_do_cartucho")]
    [InlineData(PragaTipo.MoscaBranca, "mosca_branca")]
    [InlineData(PragaTipo.BrocaDoCafe, "broca_do_cafe")]
    public void EnumHelper_ToSnakeCase_DeveConverterPragaTipo(PragaTipo praga, string esperado)
    {
        var resultado = EnumHelper.ToSnakeCase(praga.ToString());
        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void EnumHelper_PragaLabel_DeveRetornarLabelPtBr()
    {
        Assert.Equal("Ferrugem Asiática", EnumHelper.PragaLabel(PragaTipo.FerrugemAsiatica));
        Assert.Equal("Lagarta-do-cartucho", EnumHelper.PragaLabel(PragaTipo.LagartaDoCartucho));
        Assert.Equal("Broca-do-café", EnumHelper.PragaLabel(PragaTipo.BrocaDoCafe));
        Assert.Equal("Oídio", EnumHelper.PragaLabel(PragaTipo.Oidio));
    }

    [Fact]
    public void EnumHelper_FromSnakeCase_DeveConverterNivelSeveridade()
    {
        Assert.Equal(NivelSeveridade.Baixo, EnumHelper.FromSnakeCase<NivelSeveridade>("baixo"));
        Assert.Equal(NivelSeveridade.Medio, EnumHelper.FromSnakeCase<NivelSeveridade>("medio"));
        Assert.Equal(NivelSeveridade.Alto, EnumHelper.FromSnakeCase<NivelSeveridade>("alto"));
        Assert.Equal(NivelSeveridade.Critico, EnumHelper.FromSnakeCase<NivelSeveridade>("critico"));
    }

    [Fact]
    public void EnumHelper_FromSnakeCase_DeveLancarExcecaoParaValorInvalido()
    {
        Assert.Throws<RegraDeNegocioException>(() =>
            EnumHelper.FromSnakeCase<PragaTipo>("praga_inexistente"));
    }
}
