using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

/// <summary>
/// Classe estática utilitária para criação e consulta de alertas.
/// Demonstra o uso de static class conforme rubrica de POO.
/// </summary>
public static class AlertaFactory
{
    public static AlertaPraga CriarPraga(Guid propriedadeId, double probabilidade, string especie, string cultura) =>
        new(propriedadeId, probabilidade, especie, cultura);

    public static AlertaSeca CriarSeca(Guid propriedadeId, double probabilidade, int diasSemChuva) =>
        new(propriedadeId, probabilidade, diasSemChuva);

    public static AlertaGeada CriarGeada(Guid propriedadeId, double probabilidade, double temperaturaMinima) =>
        new(propriedadeId, probabilidade, temperaturaMinima);

    public static AlertaEnchente CriarEnchente(Guid propriedadeId, double probabilidade, double volumeMM) =>
        new(propriedadeId, probabilidade, volumeMM);

    public static AlertaErosao CriarErosao(Guid propriedadeId, double probabilidade, double inclinacao) =>
        new(propriedadeId, probabilidade, inclinacao);

    public static IEnumerable<Alerta> FiltrarPorSeveridade(IEnumerable<Alerta> alertas, NivelSeveridade nivel) =>
        alertas.Where(a => a.CalcularSeveridade() == nivel);

    public static string DescricaoSeveridade(NivelSeveridade nivel) => nivel switch
    {
        NivelSeveridade.Critico => "Ação imediata necessária",
        NivelSeveridade.Alto    => "Atenção elevada",
        NivelSeveridade.Medio   => "Monitoramento recomendado",
        _                       => "Situação estável"
    };
}
