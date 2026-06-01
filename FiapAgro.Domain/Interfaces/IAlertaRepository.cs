using FiapAgro.Domain.Entities;

namespace FiapAgro.Domain.Interfaces;

public interface IAlertaRepository
{
    Task<Alerta?> BuscarPorIdAsync(Guid id);
    Task<IEnumerable<Alerta>> ListarPorPropriedadeAsync(Guid propriedadeId);
    Task AdicionarAsync(Alerta alerta);
    Task<IEnumerable<Alerta>> ListarRecentesAsync(int quantidade = 20);
}
