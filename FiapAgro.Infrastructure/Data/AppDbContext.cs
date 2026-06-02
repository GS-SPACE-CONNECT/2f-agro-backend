using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FiapAgro.Infrastructure.Data;

/// <summary>
/// Contexto principal do EF Core. Mapeia as entidades do domínio para o banco PostgreSQL
/// e configura a hierarquia de <see cref="Alerta"/> usando Table-Per-Hierarchy (TPH),
/// mantendo todos os tipos de alerta em uma única tabela com coluna discriminadora.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Propriedade> Propriedades => Set<Propriedade>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarPropriedade(modelBuilder);
        ConfigurarAlerta(modelBuilder);
        ConfigurarUsuario(modelBuilder);
    }

    private static void ConfigurarPropriedade(ModelBuilder mb)
    {
        mb.Entity<Propriedade>(entity =>
        {
            entity.ToTable("propriedades");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Nome).HasMaxLength(200).IsRequired();
            entity.Property(p => p.Municipio).HasMaxLength(150).IsRequired();
            entity.Property(p => p.Estado).HasMaxLength(2).IsRequired();
            entity.Property(p => p.AreaHectares).IsRequired();
            entity.Property(p => p.CriadoEm).IsRequired();

            // Coordenada é um readonly struct (value object); ComplexProperty mapeia
            // Latitude e Longitude como colunas embutidas na tabela proprietária,
            // sem chave própria e sem tabela separada.
            entity.ComplexProperty(p => p.Localizacao, c =>
            {
                c.Property(l => l.Latitude).HasColumnName("localizacao_lat").HasDefaultValue(0.0);
                c.Property(l => l.Longitude).HasColumnName("localizacao_lng").HasDefaultValue(0.0);
            });
        });
    }

    private static void ConfigurarUsuario(ModelBuilder mb)
    {
        mb.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nome).HasMaxLength(150).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(200).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.SenhaHash).IsRequired();
            entity.Property(u => u.CriadoEm).IsRequired();
        });
    }

    private static void ConfigurarAlerta(ModelBuilder mb)
    {
        mb.Entity<Alerta>(entity =>
        {
            entity.ToTable("alertas");
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Probabilidade).IsRequired();
            entity.Property(a => a.CriadoEm).IsRequired();
            entity.Property(a => a.PropriedadeId).IsRequired();

            // TPH: todos os subtipos na mesma tabela; coluna discriminadora identifica o tipo.
            entity.HasDiscriminator<string>("tipo_alerta")
                .HasValue<AlertaPraga>("Praga")
                .HasValue<AlertaSeca>("Seca")
                .HasValue<AlertaGeada>("Geada")
                .HasValue<AlertaEnchente>("Enchente")
                .HasValue<AlertaErosao>("Erosao");

            entity.Property<string>("tipo_alerta").HasMaxLength(20).IsRequired();
        });

        mb.Entity<AlertaPraga>(entity =>
        {
            entity.Property(a => a.EspeciePraga).HasMaxLength(200);
            entity.Property(a => a.CulturaAfetada).HasMaxLength(200);
        });

        mb.Entity<AlertaSeca>(entity =>
        {
            entity.Property(a => a.DiasSemChuva);
        });

        mb.Entity<AlertaGeada>(entity =>
        {
            entity.Property(a => a.TemperaturaMinima);
        });

        mb.Entity<AlertaEnchente>(entity =>
        {
            entity.Property(a => a.VolumeMM);
        });

        mb.Entity<AlertaErosao>(entity =>
        {
            entity.Property(a => a.InclinacaoSolo);
        });
    }
}
