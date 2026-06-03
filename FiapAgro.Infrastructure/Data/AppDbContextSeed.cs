using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;
using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Infrastructure.Data;

/// <summary>
/// Popula o banco com dados de teste representativos do domínio agroclimático.
/// É idempotente: não insere registros se a tabela de propriedades já tiver dados.
/// </summary>
public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Propriedades.Any())
            return;

        var usuarioId = Guid.NewGuid();

        var fazendaABC = new Propriedade(
            nome: "Fazenda ABC",
            municipio: "Ribeirão Preto",
            estado: "SP",
            areaHectares: 450.0,
            usuarioId: usuarioId,
            localizacao: new Coordenada(-21.1784, -47.8108));

        var sitioVerde = new Propriedade(
            nome: "Sítio Verde",
            municipio: "Ponta Grossa",
            estado: "PR",
            areaHectares: 120.5,
            usuarioId: usuarioId,
            localizacao: new Coordenada(-25.0916, -50.1619));

        db.Propriedades.AddRange(fazendaABC, sitioVerde);
        await db.SaveChangesAsync();

        var alertas = new List<Alerta>
        {
            new AlertaPraga(fazendaABC.Id, 0.87, "Spodoptera frugiperda", "Milho"),
            new AlertaSeca(fazendaABC.Id, 0.72, diasSemChuva: 18),
            new AlertaGeada(sitioVerde.Id, 0.65, temperaturaMinima: -1.5),
            new AlertaEnchente(sitioVerde.Id, 0.91, volumeMM: 130),
            new AlertaErosao(fazendaABC.Id, 0.55, inclinacaoSolo: 22.0),
        };

        db.Alertas.AddRange(alertas);
        await db.SaveChangesAsync();

        // --- Lavouras ---
        var milhoL1 = new Lavoura(
            fazendaABC.Id, CulturaTipo.Milho, "L1", 80.0,
            SaudeLavoura.Saudavel, ndviAtual: 0.72,
            ultimaLeitura: DateTime.UtcNow.AddHours(-6),
            coordenadas: new Coordenada(-21.1790, -47.8115));

        var sojaL2 = new Lavoura(
            fazendaABC.Id, CulturaTipo.Soja, "L2", 120.0,
            SaudeLavoura.Atencao, ndviAtual: 0.58,
            ultimaLeitura: DateTime.UtcNow.AddHours(-12),
            coordenadas: new Coordenada(-21.1800, -47.8095));

        var canaL3 = new Lavoura(
            fazendaABC.Id, CulturaTipo.Cana, "L3", 200.0,
            SaudeLavoura.Saudavel, ndviAtual: 0.75,
            ultimaLeitura: DateTime.UtcNow.AddDays(-1),
            coordenadas: new Coordenada(-21.1775, -47.8130));

        var feijaoL1 = new Lavoura(
            sitioVerde.Id, CulturaTipo.Feijao, "L1", 40.0,
            SaudeLavoura.Saudavel, ndviAtual: 0.68,
            ultimaLeitura: DateTime.UtcNow.AddHours(-8),
            coordenadas: new Coordenada(-25.0920, -50.1625));

        var tomateL2 = new Lavoura(
            sitioVerde.Id, CulturaTipo.Tomate, "L2", 30.0,
            SaudeLavoura.Risco, ndviAtual: 0.35,
            ultimaLeitura: DateTime.UtcNow.AddHours(-3),
            coordenadas: new Coordenada(-25.0910, -50.1610));

        var alfaceL3 = new Lavoura(
            sitioVerde.Id, CulturaTipo.Alface, "L3", 20.0,
            SaudeLavoura.Saudavel, ndviAtual: 0.71,
            ultimaLeitura: DateTime.UtcNow.AddHours(-5),
            coordenadas: new Coordenada(-25.0925, -50.1630));

        db.Lavouras.AddRange(milhoL1, sojaL2, canaL3, feijaoL1, tomateL2, alfaceL3);
        await db.SaveChangesAsync();

        // --- Diagnósticos de praga ---
        var diagnosticos = new List<DiagnosticoPraga>
        {
            new DiagnosticoPraga(
                fotoUrl: "/uploads/diagnosticos/seed-tomate-ferrugem.jpg",
                praga: PragaTipo.FerrugemAsiatica,
                confianca: 0.87,
                severidade: NivelSeveridade.Alto,
                recomendacao: "Aplicar fungicida sistêmico e monitorar folhas adjacentes.",
                agronomoTelefone: "+5541999990001",
                lavouraId: tomateL2.Id),

            new DiagnosticoPraga(
                fotoUrl: "/uploads/diagnosticos/seed-milho-sadia.jpg",
                praga: PragaTipo.Sadia,
                confianca: 0.91,
                severidade: NivelSeveridade.Baixo,
                recomendacao: "Planta saudável. Manter manejo preventivo.",
                agronomoTelefone: "+5516999990002",
                lavouraId: milhoL1.Id),
        };

        db.Diagnosticos.AddRange(diagnosticos);
        await db.SaveChangesAsync();
    }
}
