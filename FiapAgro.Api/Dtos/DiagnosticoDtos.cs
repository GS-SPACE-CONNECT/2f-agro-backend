using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Helpers;

namespace FiapAgro.Api.Dtos;

public record DiagnosticoRequest(
    Guid? LavouraId = null,
    string? Praga = null,
    double? Confianca = null,
    string? Severidade = null,
    string? Recomendacao = null,
    string? AgronomoTelefone = null);

public record DiagnosticoResponse(
    Guid Id,
    Guid? LavouraId,
    string FotoUri,
    string Praga,
    string PragaLabel,
    double Confianca,
    string Severidade,
    string Recomendacao,
    string AgronomoTelefone,
    string CriadoEm)
{
    public static DiagnosticoResponse FromEntity(DiagnosticoPraga d) => new(
        d.Id,
        d.LavouraId,
        d.FotoUrl,
        EnumHelper.ToSnakeCase(d.Praga.ToString()),
        EnumHelper.PragaLabel(d.Praga),
        d.Confianca,
        EnumHelper.ToSnakeCase(d.Severidade.ToString()),
        d.Recomendacao,
        d.AgronomoTelefone,
        d.CriadoEm.ToString("o"));
}
