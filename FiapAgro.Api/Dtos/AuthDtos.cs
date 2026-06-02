namespace FiapAgro.Api.Dtos;

public record RegistrarRequest(string Nome, string Email, string Senha);

public record LoginRequest(string Email, string Senha);

public record TokenResponse(string Token, string Nome, string Email, DateTime Expira);
