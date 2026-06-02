namespace FiapAgro.Domain.Entities;

/// <summary>
/// Representa um usuário autenticável do sistema.
/// A senha nunca é armazenada em texto puro — apenas o hash PBKDF2.
/// </summary>
public class Usuario
{
    public Guid Id { get; private set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string SenhaHash { get; set; }
    public DateTime CriadoEm { get; private set; }

    // Construtor para EF Core.
    private Usuario() { Nome = string.Empty; Email = string.Empty; SenhaHash = string.Empty; }

    public Usuario(string nome, string email, string senhaHash)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Email = email.ToLowerInvariant();
        SenhaHash = senhaHash;
        CriadoEm = DateTime.UtcNow;
    }
}
