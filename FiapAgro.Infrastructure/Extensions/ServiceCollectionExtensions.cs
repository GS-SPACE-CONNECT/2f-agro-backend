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
        // PostgreSQL via EF Core — connection string lida de appsettings.json
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Default")));

        // Repositórios in-memory (substituídos por implementações EF quando necessário)
        services.AddSingleton<IPropriedadeRepository, PropriedadeRepositoryInMemory>();
        services.AddSingleton<IAlertaRepository, AlertaRepositoryInMemory>();

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
