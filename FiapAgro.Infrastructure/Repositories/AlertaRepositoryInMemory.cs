using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;

namespace FiapAgro.Infrastructure.Repositories;

public class AlertaRepositoryInMemory : IAlertaRepository
{
    private readonly List<Alerta> _store = new();

    public Task<Alerta?> BuscarPorIdAsync(Guid id) =>
        Task.FromResult(_store.FirstOrDefault(a => a.Id == id));

    public Task<IEnumerable<Alerta>> ListarPorPropriedadeAsync(Guid propriedadeId) =>
        Task.FromResult(_store.Where(a => a.PropriedadeId == propriedadeId));

    public Task AdicionarAsync(Alerta alerta)
    {
        _store.Add(alerta);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Alerta>> ListarRecentesAsync(int quantidade = 20) =>
        Task.FromResult(_store.OrderByDescending(a => a.CriadoEm).Take(quantidade));

    public Task<IEnumerable<Alerta>> ListarPorPeriodoAsync(
        Guid propriedadeId, DateTime inicio, DateTime fim) =>
        Task.FromResult<IEnumerable<Alerta>>(_store
            .Where(a => a.PropriedadeId == propriedadeId
                        && a.CriadoEm >= inicio
                        && a.CriadoEm <= fim)
            .OrderByDescending(a => a.CriadoEm));
}
