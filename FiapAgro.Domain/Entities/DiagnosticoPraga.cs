using FiapAgro.Domain.Enums;

namespace FiapAgro.Domain.Entities;

/// <summary>
/// Resultado de um diagnóstico de praga via visão computacional ("Olho na Folha").
/// Pode estar vinculado a uma lavoura (opcional) e armazena a foto, tipo de praga detectada,
/// confiança do modelo e recomendação para o agricultor.
/// </summary>
public class DiagnosticoPraga
{
    public Guid Id { get; private set; }
    public Guid? LavouraId { get; set; }
    public string FotoUrl { get; set; }
    public PragaTipo Praga { get; set; }
    public double Confianca { get; set; }
    public NivelSeveridade Severidade { get; set; }
    public string Recomendacao { get; set; }
    public string AgronomoTelefone { get; set; }
    public DateTime CriadoEm { get; private set; }

    // Construtor sem parâmetros exigido pelo EF Core para materialização via reflexão.
    private DiagnosticoPraga()
    {
        FotoUrl = string.Empty;
        Recomendacao = string.Empty;
        AgronomoTelefone = string.Empty;
    }

    public DiagnosticoPraga(
        string fotoUrl,
        PragaTipo praga,
        double confianca,
        NivelSeveridade severidade,
        string recomendacao,
        string agronomoTelefone,
        Guid? lavouraId = null)
    {
        Id = Guid.NewGuid();
        FotoUrl = fotoUrl;
        Praga = praga;
        Confianca = Math.Clamp(confianca, 0.0, 1.0);
        Severidade = severidade;
        Recomendacao = recomendacao;
        AgronomoTelefone = agronomoTelefone;
        LavouraId = lavouraId;
        CriadoEm = DateTime.UtcNow;
    }
}
