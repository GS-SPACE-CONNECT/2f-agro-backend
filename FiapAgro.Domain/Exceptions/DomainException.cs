namespace FiapAgro.Domain.Exceptions;

/// <summary>
/// Classe base para todas as exceções de domínio da aplicação.
/// Permite que o middleware global distinga erros esperados (de negócio)
/// de erros inesperados (infraestrutura/runtime).
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
