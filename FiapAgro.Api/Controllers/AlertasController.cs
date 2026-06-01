using FiapAgro.Api.Dtos;
using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertasController : ControllerBase
{
    private readonly IAlertaRepository _repo;
    private readonly ILogger<AlertasController> _logger;

    public AlertasController(IAlertaRepository repo, ILogger<AlertasController> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    /// <summary>Lista os alertas mais recentes.</summary>
    [HttpGet("recentes")]
    public async Task<IActionResult> Recentes([FromQuery] int quantidade = 20)
    {
        try
        {
            var alertas = await _repo.ListarRecentesAsync(quantidade);
            return Ok(alertas.Select(AlertaResponse.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar alertas recentes.");
            return StatusCode(500, "Erro interno ao listar alertas.");
        }
    }

    /// <summary>Lista alertas de uma propriedade.</summary>
    [HttpGet("propriedade/{propriedadeId:guid}")]
    public async Task<IActionResult> PorPropriedade(Guid propriedadeId)
    {
        try
        {
            var alertas = await _repo.ListarPorPropriedadeAsync(propriedadeId);
            return Ok(alertas.Select(AlertaResponse.FromEntity));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar alertas da propriedade {Id}.", propriedadeId);
            return StatusCode(500, "Erro interno ao listar alertas.");
        }
    }

    /// <summary>Retorna um alerta pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        try
        {
            var alerta = await _repo.BuscarPorIdAsync(id);
            if (alerta is null)
                return NotFound($"Alerta {id} não encontrado.");

            return Ok(AlertaResponse.FromEntity(alerta));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar alerta {Id}.", id);
            return StatusCode(500, "Erro interno ao buscar alerta.");
        }
    }

    /// <summary>Registra alerta de praga.</summary>
    [HttpPost("praga")]
    public async Task<IActionResult> CriarPraga([FromBody] AlertaPragaRequest request)
        => await CriarAlerta(
            new AlertaPraga(request.PropriedadeId, request.Probabilidade,
                request.EspeciePraga, request.CulturaAfetada));

    /// <summary>Registra alerta de seca.</summary>
    [HttpPost("seca")]
    public async Task<IActionResult> CriarSeca([FromBody] AlertaSecaRequest request)
        => await CriarAlerta(
            new AlertaSeca(request.PropriedadeId, request.Probabilidade, request.DiasSemChuva));

    /// <summary>Registra alerta de geada.</summary>
    [HttpPost("geada")]
    public async Task<IActionResult> CriarGeada([FromBody] AlertaGeadaRequest request)
        => await CriarAlerta(
            new AlertaGeada(request.PropriedadeId, request.Probabilidade, request.TemperaturaMinima));

    /// <summary>Registra alerta de enchente.</summary>
    [HttpPost("enchente")]
    public async Task<IActionResult> CriarEnchente([FromBody] AlertaEnchenteRequest request)
        => await CriarAlerta(
            new AlertaEnchente(request.PropriedadeId, request.Probabilidade, request.VolumeMM));

    /// <summary>Registra alerta de erosão.</summary>
    [HttpPost("erosao")]
    public async Task<IActionResult> CriarErosao([FromBody] AlertaErosaoRequest request)
        => await CriarAlerta(
            new AlertaErosao(request.PropriedadeId, request.Probabilidade, request.InclinacaoSolo));

    private async Task<IActionResult> CriarAlerta(Alerta alerta)
    {
        try
        {
            await _repo.AdicionarAsync(alerta);
            _logger.LogInformation("Alerta {Tipo} criado para propriedade {Id}.",
                alerta.GetType().Name, alerta.PropriedadeId);

            return CreatedAtAction(nameof(BuscarPorId), new { id = alerta.Id },
                AlertaResponse.FromEntity(alerta));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar alerta {Tipo}.", alerta.GetType().Name);
            return StatusCode(500, "Erro interno ao criar alerta.");
        }
    }
}
