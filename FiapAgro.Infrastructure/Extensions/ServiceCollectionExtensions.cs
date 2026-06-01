using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Detectors;
using FiapAgro.Infrastructure.Notifications;
using FiapAgro.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace FiapAgro.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFiapAgroServices(this IServiceCollection services)
    {
        // Repositórios
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
