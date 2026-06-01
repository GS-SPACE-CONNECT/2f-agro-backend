using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;

namespace FiapAgro.Infrastructure.Repositories;

public class PropriedadeRepositoryInMemory : IPropriedadeRepository
{
    private readonly List<Propriedade> _store = new();

    public Task<Propriedade?> BuscarPorIdAsync(Guid id) =>
        Task.FromResult(_store.FirstOrDefault(p => p.Id == id));

    public Task<IEnumerable<Propriedade>> ListarPorUsuarioAsync(Guid usuarioId) =>
        Task.FromResult(_store.Where(p => p.UsuarioId == usuarioId));

    public Task AdicionarAsync(Propriedade propriedade)
    {
        _store.Add(propriedade);
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Propriedade propriedade)
    {
        var idx = _store.FindIndex(p => p.Id == propriedade.Id);
        if (idx >= 0) _store[idx] = propriedade;
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Guid id)
    {
        _store.RemoveAll(p => p.Id == id);
        return Task.CompletedTask;
    }
}
