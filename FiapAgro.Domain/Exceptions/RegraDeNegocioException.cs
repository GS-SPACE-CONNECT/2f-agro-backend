namespace FiapAgro.Domain.Exceptions;

/// <summary>Violação de regra de negócio ou entrada inválida → HTTP 400.</summary>
public sealed class RegraDeNegocioException : DomainException
{
    public RegraDeNegocioException(string message) : base(message) { }
}
