using FiapAgro.Api.Dtos;
using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropriedadesController : ControllerBase
{
    private readonly IPropriedadeRepository _repo;
    private readonly ILogger<PropriedadesController> _logger;

    public PropriedadesController(IPropriedadeRepository repo, ILogger<PropriedadesController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    /// <summary>Lista propriedades de um usuário.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid usuarioId)
    {
        try
        {
            var propriedades = await _repo.ListarPorUsuarioAsync(usuarioId);
            return Ok(propriedades.Select(PropriedadeResponse.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar propriedades do usuário {UsuarioId}.", usuarioId);
            return StatusCode(500, "Erro interno ao listar propriedades.");
        }
    }

    /// <summary>Retorna uma propriedade pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        try
        {
            var propriedade = await _repo.BuscarPorIdAsync(id);
            if (propriedade is null)
                return NotFound($"Propriedade {id} não encontrada.");

            return Ok(PropriedadeResponse.FromEntity(propriedade));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar propriedade {Id}.", id);
            return StatusCode(500, "Erro interno ao buscar propriedade.");
        }
    }

    /// <summary>Cadastra uma nova propriedade.</summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PropriedadeRequest request)
    {
        try
        {
            var localizacao = request.Latitude.HasValue && request.Longitude.HasValue
                ? new Coordenada(request.Latitude.Value, request.Longitude.Value)
                : Coordenada.Vazia;

            var propriedade = new Propriedade(
                request.Nome, request.Municipio, request.Estado,
                request.AreaHectares, request.UsuarioId, localizacao);

            await _repo.AdicionarAsync(propriedade);
            _logger.LogInformation("Propriedade {Nome} criada com Id {Id}.", propriedade.Nome, propriedade.Id);

            return CreatedAtAction(nameof(BuscarPorId), new { id = propriedade.Id },
                PropriedadeResponse.FromEntity(propriedade));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar propriedade.");
            return StatusCode(500, "Erro interno ao criar propriedade.");
        }
    }

    /// <summary>Atualiza dados de uma propriedade existente.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PropriedadeRequest request)
    {
        try
        {
            var propriedade = await _repo.BuscarPorIdAsync(id);
            if (propriedade is null)
                return NotFound($"Propriedade {id} não encontrada.");

            propriedade.Nome = request.Nome;
            propriedade.Municipio = request.Municipio;
            propriedade.Estado = request.Estado;
            propriedade.AreaHectares = request.AreaHectares;

            if (request.Latitude.HasValue && request.Longitude.HasValue)
                propriedade.Localizacao = new Coordenada(request.Latitude.Value, request.Longitude.Value);

            await _repo.AtualizarAsync(propriedade);
            return Ok(PropriedadeResponse.FromEntity(propriedade));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar propriedade {Id}.", id);
            return StatusCode(500, "Erro interno ao atualizar propriedade.");
        }
    }

    /// <summary>Remove uma propriedade.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        try
        {
            var propriedade = await _repo.BuscarPorIdAsync(id);
            if (propriedade is null)
                return NotFound($"Propriedade {id} não encontrada.");

            await _repo.RemoverAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover propriedade {Id}.", id);
            return StatusCode(500, "Erro interno ao remover propriedade.");
        }
    }
}
