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
}
