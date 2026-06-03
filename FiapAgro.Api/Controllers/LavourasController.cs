using FiapAgro.Api.Dtos;
using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;
using FiapAgro.Domain.Exceptions;
using FiapAgro.Domain.Helpers;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LavourasController : ControllerBase
{
    private readonly ILavouraRepository _repo;
    private readonly ILogger<LavourasController> _logger;

    public LavourasController(ILavouraRepository repo, ILogger<LavourasController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    /// <summary>Lista lavouras de uma propriedade.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid propriedadeId)
    {
        var lavouras = await _repo.ListarPorPropriedadeAsync(propriedadeId);
        return Ok(lavouras.Select(LavouraResponse.FromEntity));
    }

    /// <summary>Retorna uma lavoura pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var lavoura = await _repo.BuscarPorIdAsync(id);
        if (lavoura is null)
            throw new NaoEncontradoException("Lavoura", id);

        return Ok(LavouraResponse.FromEntity(lavoura));
    }

    /// <summary>Cadastra uma nova lavoura.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] LavouraRequest request)
    {
        var cultura = EnumHelper.FromSnakeCase<CulturaTipo>(request.Cultura);
        var saude = request.Saude is not null
            ? EnumHelper.FromSnakeCase<SaudeLavoura>(request.Saude)
            : SaudeLavoura.Saudavel;

        var coordenadas = request.Latitude.HasValue && request.Longitude.HasValue
            ? new Coordenada(request.Latitude.Value, request.Longitude.Value)
            : Coordenada.Vazia;

        var lavoura = new Lavoura(
            request.PropriedadeId,
            cultura,
            request.Identificador,
            request.AreaHectares,
            saude,
            request.NdviAtual,
            coordenadas: coordenadas);

        await _repo.AdicionarAsync(lavoura);
        _logger.LogInformation("Lavoura {Identificador} criada com Id {Id}.", lavoura.Identificador, lavoura.Id);

        return CreatedAtAction(nameof(BuscarPorId), new { id = lavoura.Id },
            LavouraResponse.FromEntity(lavoura));
    }

    /// <summary>Atualiza dados de uma lavoura existente.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] LavouraRequest request)
    {
        var lavoura = await _repo.BuscarPorIdAsync(id);
        if (lavoura is null)
            throw new NaoEncontradoException("Lavoura", id);

        lavoura.Cultura = EnumHelper.FromSnakeCase<CulturaTipo>(request.Cultura);
        lavoura.Identificador = request.Identificador;
        lavoura.AreaHectares = request.AreaHectares;
        lavoura.Saude = request.Saude is not null
            ? EnumHelper.FromSnakeCase<SaudeLavoura>(request.Saude)
            : lavoura.Saude;
        lavoura.NdviAtual = request.NdviAtual;

        if (request.Latitude.HasValue && request.Longitude.HasValue)
            lavoura.Coordenadas = new Coordenada(request.Latitude.Value, request.Longitude.Value);

        await _repo.AtualizarAsync(lavoura);
        return Ok(LavouraResponse.FromEntity(lavoura));
    }

    /// <summary>Remove uma lavoura.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var lavoura = await _repo.BuscarPorIdAsync(id);
        if (lavoura is null)
            throw new NaoEncontradoException("Lavoura", id);

        await _repo.RemoverAsync(id);
        return NoContent();
    }
}
