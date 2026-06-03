using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FiapAgro.Infrastructure.Data;

/// <summary>
/// Factory de design-time para o <see cref="AppDbContext"/>.
/// Permite que o CLI <c>dotnet ef</c> instancie o contexto sem precisar
/// inicializar o host completo da aplicação.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=fiapagro_dev;Username=postgres;Password=postgres")
            .Options;

        return new AppDbContext(options);
    }
}
