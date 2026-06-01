using FiapAgro.Domain.ValueObjects;

namespace FiapAgro.Domain.Entities;

/// <summary>
/// Representa uma propriedade rural cadastrada no sistema.
/// Declarada como <c>partial class</c> para separar responsabilidades em arquivos distintos
/// sem fragmentar a classe em tipos auxiliares desnecessários:
/// <list type="bullet">
///   <item><description><b>Propriedade.cs</b> — estado: propriedades e construtor.</description></item>
///   <item><description><b>Propriedade.Validacao.cs</b> — comportamento: métodos de consulta e validação.</description></item>
/// </list>
/// Essa divisão mantém cada arquivo coeso e facilita a navegação em codebases maiores.
/// </summary>
public partial class Propriedade
{
    public Guid Id { get; private set; }
    public string Nome { get; set; }
    public string Municipio { get; set; }
    public string Estado { get; set; }
    public double AreaHectares { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime CriadoEm { get; private set; }
    public Coordenada Localizacao { get; set; }

    public Propriedade(string nome, string municipio, string estado, double areaHectares, Guid usuarioId,
        Coordenada localizacao = default)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Municipio = municipio;
        Estado = estado;
        AreaHectares = areaHectares;
        UsuarioId = usuarioId;
        CriadoEm = DateTime.UtcNow;
        Localizacao = localizacao;
    }
}
