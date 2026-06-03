using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FiapAgro.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace FiapAgro.Infrastructure.Auth;

/// <summary>
/// Responsável por gerar tokens JWT e por fazer hash/verificação de senhas via PBKDF2,
/// mantendo a lógica de segurança isolada da camada de domínio.
/// </summary>
public class JwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiresHours;

    public JwtService(IConfiguration config)
    {
        _secretKey = config["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");
        _issuer    = config["Jwt:Issuer"]    ?? "FiapAgro.Api";
        _audience  = config["Jwt:Audience"]  ?? "FiapAgro.Clients";
        _expiresHours = int.TryParse(config["Jwt:ExpiresHours"], out var h) ? h : 8;
    }

    public DateTime CalcularExpiracao() => DateTime.UtcNow.AddHours(_expiresHours);

    public string GerarToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_expiresHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashSenha(string senha)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            senha, salt, iterations: 10_000, HashAlgorithmName.SHA256, outputLength: 32);

        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    public bool VerificarSenha(string senha, string senhaHash)
    {
        var parts = senhaHash.Split(':');
        if (parts.Length != 2) return false;

        byte[] salt         = Convert.FromBase64String(parts[0]);
        byte[] expectedHash = Convert.FromBase64String(parts[1]);
        byte[] actualHash   = Rfc2898DeriveBytes.Pbkdf2(
            senha, salt, iterations: 10_000, HashAlgorithmName.SHA256, outputLength: 32);

        return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
    }
}
