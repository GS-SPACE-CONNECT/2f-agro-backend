namespace FiapAgro.Domain.Exceptions;

/// <summary>Recurso solicitado não existe na base de dados → HTTP 404.</summary>
public sealed class NaoEncontradoException : DomainException
{
    public NaoEncontradoException(string entidade, object id)
        : base($"{entidade} '{id}' não encontrado.") { }
}
