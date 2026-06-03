using FiapAgro.Api.Dtos;
using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;
using FiapAgro.Domain.Exceptions;
using FiapAgro.Domain.Helpers;
using FiapAgro.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiagnosticosController : ControllerBase
{
    private readonly IDiagnosticoRepository _repo;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DiagnosticosController> _logger;

    private static readonly string[] ExtensoesPermitidas = [".jpg", ".jpeg", ".png"];
    private const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5 MB

    // Dados mock para simulação de inferência (Sprint 1)
    private static readonly (PragaTipo Praga, double Confianca, NivelSeveridade Severidade, string Recomendacao)[] MockInferencias =
    [
        (PragaTipo.FerrugemAsiatica, 0.82, NivelSeveridade.Alto, "Aplicar fungicida sistêmico e monitorar folhas adjacentes."),
        (PragaTipo.LagartaDoCartucho, 0.76, NivelSeveridade.Alto, "Aplicar inseticida biológico (Bt) nas áreas afetadas."),
        (PragaTipo.ManchaFoliar, 0.68, NivelSeveridade.Medio, "Remover folhas infectadas e aplicar fungicida preventivo."),
        (PragaTipo.Sadia, 0.93, NivelSeveridade.Baixo, "Planta saudável. Manter manejo preventivo."),
        (PragaTipo.Oidio, 0.71, NivelSeveridade.Medio, "Aplicar fungicida à base de enxofre nas folhas afetadas."),
        (PragaTipo.MoscaBranca, 0.65, NivelSeveridade.Medio, "Instalar armadilhas adesivas amarelas e aplicar óleo de neem."),
    ];

    public DiagnosticosController(
        IDiagnosticoRepository repo,
        IWebHostEnvironment env,
        ILogger<DiagnosticosController> logger)
    {
        _repo = repo;
        _env = env;
        _logger = logger;
    }

    /// <summary>Retorna um diagnóstico pelo Id.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> BuscarPorId(Guid id)
    {
        var diagnostico = await _repo.BuscarPorIdAsync(id);
        if (diagnostico is null)
            throw new NaoEncontradoException("Diagnóstico", id);

        return Ok(DiagnosticoResponse.FromEntity(diagnostico));
    }

    /// <summary>Lista diagnósticos de uma lavoura.</summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] Guid? lavouraId)
    {
        if (lavouraId.HasValue)
        {
            var diagnosticos = await _repo.ListarPorLavouraAsync(lavouraId.Value);
            return Ok(diagnosticos.Select(DiagnosticoResponse.FromEntity));
        }

        var recentes = await _repo.ListarRecentesAsync(10);
        return Ok(recentes.Select(DiagnosticoResponse.FromEntity));
    }

    /// <summary>Lista diagnósticos recentes.</summary>
    [HttpGet("recentes")]
    public async Task<IActionResult> Recentes([FromQuery] int quantidade = 10)
    {
        var diagnosticos = await _repo.ListarRecentesAsync(quantidade);
        return Ok(diagnosticos.Select(DiagnosticoResponse.FromEntity));
    }

    /// <summary>
    /// Recebe upload de foto e cria diagnóstico.
    /// Aceita resultado pré-computado (cenário IoT/CV) ou simula inferência mock (Sprint 1).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar(
        [FromForm] IFormFile foto,
        [FromForm] Guid? lavouraId = null,
        [FromForm] string? praga = null,
        [FromForm] double? confianca = null,
        [FromForm] string? severidade = null,
        [FromForm] string? recomendacao = null,
        [FromForm] string? agronomoTelefone = null)
    {
        // Validar foto
        if (foto is null || foto.Length == 0)
            throw new RegraDeNegocioException("A foto é obrigatória.");

        if (foto.Length > TamanhoMaximoBytes)
            throw new RegraDeNegocioException("A foto deve ter no máximo 5 MB.");

        var extensao = Path.GetExtension(foto.FileName).ToLowerInvariant();
        if (!ExtensoesPermitidas.Contains(extensao))
            throw new RegraDeNegocioException("Formato inválido. Envie .jpg, .jpeg ou .png.");

        // Salvar foto em disco
        var uploadsDir = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            "uploads", "diagnosticos");
        Directory.CreateDirectory(uploadsDir);

        var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
        var caminhoFisico = Path.Combine(uploadsDir, nomeArquivo);

        await using (var stream = new FileStream(caminhoFisico, FileMode.Create))
        {
            await foto.CopyToAsync(stream);
        }

        var fotoUrl = $"/uploads/diagnosticos/{nomeArquivo}";

        // Determinar resultado: pré-computado ou mock
        PragaTipo pragaTipo;
        double confiancaFinal;
        NivelSeveridade severidadeFinal;
        string recomendacaoFinal;
        string telefoneFinal;

        if (!string.IsNullOrWhiteSpace(praga))
        {
            // Resultado pré-computado (IoT/CV)
            pragaTipo = EnumHelper.FromSnakeCase<PragaTipo>(praga);
            confiancaFinal = confianca ?? 0.5;
            severidadeFinal = severidade is not null
                ? EnumHelper.FromSnakeCase<NivelSeveridade>(severidade)
                : NivelSeveridade.Medio;
            recomendacaoFinal = recomendacao ?? "Consulte um agrônomo para orientação detalhada.";
            telefoneFinal = agronomoTelefone ?? "+5511999990000";
        }
        else
        {
            // Simulação mock (Sprint 1)
            var mock = MockInferencias[Random.Shared.Next(MockInferencias.Length)];
            pragaTipo = mock.Praga;
            confiancaFinal = mock.Confianca;
            severidadeFinal = mock.Severidade;
            recomendacaoFinal = mock.Recomendacao;
            telefoneFinal = agronomoTelefone ?? "+5511999990000";
        }

        var diagnostico = new DiagnosticoPraga(
            fotoUrl, pragaTipo, confiancaFinal, severidadeFinal,
            recomendacaoFinal, telefoneFinal, lavouraId);

        await _repo.AdicionarAsync(diagnostico);
        _logger.LogInformation("Diagnóstico {Praga} criado com Id {Id}.", pragaTipo, diagnostico.Id);

        return CreatedAtAction(nameof(BuscarPorId), new { id = diagnostico.Id },
            DiagnosticoResponse.FromEntity(diagnostico));
    }
}
