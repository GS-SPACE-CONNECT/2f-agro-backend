namespace FiapAgro.Domain.Entities;

public class Propriedade
{
    public Guid Id { get; private set; }
    public string Nome { get; set; }
    public string Municipio { get; set; }
    public string Estado { get; set; }
    public double AreaHectares { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime CriadoEm { get; private set; }

    public Propriedade(string nome, string municipio, string estado, double areaHectares, Guid usuarioId)
    {
        Id = Guid.NewGuid();
        Nome = nome;
        Municipio = municipio;
        Estado = estado;
        AreaHectares = areaHectares;
        UsuarioId = usuarioId;
        CriadoEm = DateTime.UtcNow;
    }
}
