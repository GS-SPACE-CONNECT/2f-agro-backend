using FiapAgro.Domain.Entities;

namespace FiapAgro.Domain.Interfaces;

public interface IPropriedadeRepository
{
    Task<Propriedade?> BuscarPorIdAsync(Guid id);
    Task<IEnumerable<Propriedade>> ListarPorUsuarioAsync(Guid usuarioId);
    Task AdicionarAsync(Propriedade propriedade);
    Task AtualizarAsync(Propriedade propriedade);
    Task RemoverAsync(Guid id);
}
