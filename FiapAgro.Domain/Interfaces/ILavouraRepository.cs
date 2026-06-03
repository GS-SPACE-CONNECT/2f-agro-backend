using FiapAgro.Domain.Entities;

namespace FiapAgro.Domain.Interfaces;

public interface ILavouraRepository
{
    Task<Lavoura?> BuscarPorIdAsync(Guid id);
    Task<IEnumerable<Lavoura>> ListarPorPropriedadeAsync(Guid propriedadeId);
    Task AdicionarAsync(Lavoura lavoura);
    Task AtualizarAsync(Lavoura lavoura);
    Task RemoverAsync(Guid id);
}
