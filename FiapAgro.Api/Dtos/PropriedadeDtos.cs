using FiapAgro.Domain.Entities;
using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Api.Dtos;

public record PropriedadeRequest(
    string Nome,
    string Municipio,
    string Estado,
    double AreaHectares,
    Guid UsuarioId,
    double? Latitude = null,
    double? Longitude = null);

public record PropriedadeResponse(
    Guid Id,
    string Nome,
    string Municipio,
    string Estado,
    double AreaHectares,
    Guid UsuarioId,
    string Descricao,
    DateTime CriadoEm)
{
    public static PropriedadeResponse FromEntity(Propriedade p) => new(
        p.Id, p.Nome, p.Municipio, p.Estado, p.AreaHectares,
        p.UsuarioId, p.Descricao(), p.CriadoEm);
}
