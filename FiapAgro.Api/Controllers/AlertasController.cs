using FiapAgro.Api.Dtos;
using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Exceptions;
using FiapAgro.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var alertas = await _repo.ListarRecentesAsync(quantidade);
        return Ok(alertas.Select(AlertaResponse.FromEntity));
    }

    /// <summary>Lista alertas de uma propriedade.</summary>
    [HttpGet("propriedade/{propriedadeId:guid}")]
    public async Task<IActionResult> PorPropriedade(Guid propriedadeId)
    {
        var alertas = await _repo.ListarPorPropriedadeAsync(propriedadeId);
        return Ok(alertas.Select(AlertaResponse.FromEntity));
    }

    /// <summary>
    /// Retorna o histórico de alertas de uma propriedade dentro de um período.
    /// Datas no formato ISO 8601 (ex.: 2026-01-01T00:00:00Z).
    /// </summary>
    [HttpGet("propriedade/{propriedadeId:guid}/historico")]
    public async Task<IActionResult> Historico(
        Guid propriedadeId,
        [FromQuery] DateTime inicio,
        [FromQuery] DateTime fim)
    {
        if (fim < inicio)
            throw new RegraDeNegocioException("A data final não pode ser anterior à data inicial.");

        var inicioUtc = inicio.Kind == DateTimeKind.Utc ? inicio : inicio.ToUniversalTime();
        var fimUtc = fim.Kind == DateTimeKind.Utc ? fim : fim.ToUniversalTime();

        var alertas = await _repo.ListarPorPeriodoAsync(propriedadeId, inicioUtc, fimUtc);
        return Ok(alertas.Select(AlertaResponse.FromEntity));
    }

    /// <summary>Retorna um alerta pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var alerta = await _repo.BuscarPorIdAsync(id);
        if (alerta is null)
            throw new NaoEncontradoException("Alerta", id);

        return Ok(AlertaResponse.FromEntity(alerta));
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
        await _repo.AdicionarAsync(alerta);
        _logger.LogInformation("Alerta {Tipo} criado para propriedade {Id}.",
            alerta.GetType().Name, alerta.PropriedadeId);

        return CreatedAtAction(nameof(BuscarPorId), new { id = alerta.Id },
            AlertaResponse.FromEntity(alerta));
    }
}
