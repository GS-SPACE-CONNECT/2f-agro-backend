using FiapAgro.Domain.Entities;
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
    }
}
