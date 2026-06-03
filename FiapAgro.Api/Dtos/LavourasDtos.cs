using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Helpers;

namespace FiapAgro.Api.Dtos;

public record LavouraRequest(
    Guid PropriedadeId,
    string Cultura,
    string Identificador,
    double AreaHectares,
    string? Saude = null,
    double? NdviAtual = null,
    double? Latitude = null,
    double? Longitude = null);

public record CoordenadaDto(double Lat, double Lng);

public record LavouraResponse(
    Guid Id,
    Guid PropriedadeId,
    string Cultura,
    string CulturaLabel,
    string Identificador,
    double AreaHectares,
    string Saude,
    double? NdviAtual,
    string? UltimaLeitura,
    CoordenadaDto? Coordenadas,
    string CriadoEm)
{
    public static LavouraResponse FromEntity(Lavoura l) => new(
        l.Id,
        l.PropriedadeId,
        EnumHelper.ToSnakeCase(l.Cultura.ToString()),
        EnumHelper.CulturaLabel(l.Cultura),
        l.Identificador,
        l.AreaHectares,
        EnumHelper.ToSnakeCase(l.Saude.ToString()),
        l.NdviAtual,
        l.UltimaLeitura?.ToString("o"),
        l.Coordenadas.IsValida() ? new CoordenadaDto(l.Coordenadas.Latitude, l.Coordenadas.Longitude) : null,
        l.CriadoEm.ToString("o"));
}
