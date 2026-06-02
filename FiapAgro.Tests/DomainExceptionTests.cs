using FiapAgro.Domain.Exceptions;

namespace FiapAgro.Tests;

public class DomainExceptionTests
{
    // ── Hierarquia de herança ───────────────────────────────────────────────

    [Fact]
    public void NaoEncontradoException_HerdaDeDomainException()
    {
        var ex = new NaoEncontradoException("Propriedade", Guid.NewGuid());
        Assert.IsAssignableFrom<DomainException>(ex);
        Assert.IsAssignableFrom<Exception>(ex);
    }

    [Fact]
    public void ConflitoException_HerdaDeDomainException()
    {
        var ex = new ConflitoException("E-mail já cadastrado.");
        Assert.IsAssignableFrom<DomainException>(ex);
    }

    [Fact]
    public void RegraDeNegocioException_HerdaDeDomainException()
    {
        var ex = new RegraDeNegocioException("Latitude inválida.");
        Assert.IsAssignableFrom<DomainException>(ex);
    }

    // ── Mensagens ───────────────────────────────────────────────────────────

    [Fact]
    public void NaoEncontradoException_Mensagem_ContemEntidadeEId()
    {
        var id = Guid.NewGuid();
        var ex = new NaoEncontradoException("Alerta", id);
        Assert.Contains("Alerta", ex.Message);
        Assert.Contains(id.ToString(), ex.Message);
    }

    [Fact]
    public void ConflitoException_Mensagem_ContemTextoInformado()
    {
        const string msg = "E-mail 'teste@email.com' já está cadastrado.";
        var ex = new ConflitoException(msg);
        Assert.Equal(msg, ex.Message);
    }

    [Fact]
    public void RegraDeNegocioException_Mensagem_ContemTextoInformado()
    {
        const string msg = "Longitude deve estar entre -180 e 180.";
        var ex = new RegraDeNegocioException(msg);
        Assert.Equal(msg, ex.Message);
    }

    // ── Captura polimórfica ─────────────────────────────────────────────────

    [Fact]
    public void DomainException_PodeCapturarNaoEncontrado()
    {
        static void Lancar() => throw new NaoEncontradoException("Usuario", Guid.NewGuid());
        Assert.Throws<NaoEncontradoException>(Lancar);
        Assert.ThrowsAny<DomainException>(Lancar);
    }

    [Fact]
    public void DomainException_PodeCapturarConflito()
    {
        static void Lancar() => throw new ConflitoException("Conflito.");
        Assert.Throws<ConflitoException>(Lancar);
        Assert.ThrowsAny<DomainException>(Lancar);
    }

    [Fact]
    public void DomainException_PodeCapturarRegraDeNegocio()
    {
        static void Lancar() => throw new RegraDeNegocioException("Inválido.");
        Assert.Throws<RegraDeNegocioException>(Lancar);
        Assert.ThrowsAny<DomainException>(Lancar);
    }
}
