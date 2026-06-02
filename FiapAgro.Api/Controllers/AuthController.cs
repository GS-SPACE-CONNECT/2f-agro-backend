using FiapAgro.Api.Dtos;
using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FiapAgro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _repo;
    private readonly JwtService _jwt;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUsuarioRepository repo, JwtService jwt, ILogger<AuthController> logger)
    {
        _repo   = repo;
        _jwt    = jwt;
        _logger = logger;
    }

    /// <summary>Cadastra um novo usuário e retorna o token JWT.</summary>
    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarRequest request)
    {
        try
        {
            if (await _repo.ExisteEmailAsync(request.Email))
                return Conflict($"E-mail '{request.Email}' já está cadastrado.");

            var senhaHash = _jwt.HashSenha(request.Senha);
            var usuario   = new Usuario(request.Nome, request.Email, senhaHash);

            await _repo.AdicionarAsync(usuario);
            _logger.LogInformation("Usuário {Email} registrado com Id {Id}.", usuario.Email, usuario.Id);

            return Created(string.Empty, GerarResposta(usuario));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao registrar usuário.");
            return StatusCode(500, "Erro interno ao registrar usuário.");
        }
    }

    /// <summary>Autentica um usuário e retorna o token JWT.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var usuario = await _repo.BuscarPorEmailAsync(request.Email);

            if (usuario is null || !_jwt.VerificarSenha(request.Senha, usuario.SenhaHash))
                return Unauthorized("E-mail ou senha inválidos.");

            _logger.LogInformation("Login bem-sucedido para {Email}.", usuario.Email);
            return Ok(GerarResposta(usuario));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao autenticar usuário.");
            return StatusCode(500, "Erro interno ao autenticar.");
        }
    }

    private TokenResponse GerarResposta(Usuario usuario) => new(
        Token:  _jwt.GerarToken(usuario),
        Nome:   usuario.Nome,
        Email:  usuario.Email,
        Expira: _jwt.CalcularExpiracao());
}
