using FiapAgro.Domain.Entities;

namespace FiapAgro.Domain.Interfaces;

public interface IDetector<T> where T : Alerta
{
    Task<T?> AnalisarAsync(Guid propriedadeId, IDictionary<string, object> dados);
    bool PodeAnalisar(IDictionary<string, object> dados);
}
