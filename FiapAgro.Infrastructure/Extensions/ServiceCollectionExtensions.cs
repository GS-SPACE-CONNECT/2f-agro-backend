using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Data;
using FiapAgro.Infrastructure.Detectors;
using FiapAgro.Infrastructure.Notifications;
using FiapAgro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FiapAgro.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFiapAgroServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL via EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Repositórios EF Core — Scoped para acompanhar o ciclo de vida do DbContext
        services.AddScoped<IPropriedadeRepository, PropriedadeRepositoryEF>();
        services.AddScoped<IAlertaRepository, AlertaRepositoryEF>();
        services.AddScoped<IUsuarioRepository, UsuarioRepositoryEF>();

        // Notificador
        services.AddScoped<INotificador, NotificadorConsole>();

        // Detectores — um por tipo de alerta
        services.AddScoped<IDetector<AlertaPraga>, DetectorPraga>();
        services.AddScoped<IDetector<AlertaSeca>, DetectorSeca>();
        services.AddScoped<IDetector<AlertaGeada>, DetectorGeada>();
        services.AddScoped<IDetector<AlertaEnchente>, DetectorEnchente>();
        services.AddScoped<IDetector<AlertaErosao>, DetectorErosao>();

        return services;
    }
}
