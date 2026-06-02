namespace FiapAgro.Domain.Exceptions;

/// <summary>Violação de unicidade ou conflito de estado → HTTP 409.</summary>
public sealed class ConflitoException : DomainException
{
    public ConflitoException(string message) : base(message) { }
}
