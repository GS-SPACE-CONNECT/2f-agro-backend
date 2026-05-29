using FiapAgro.Domain.Entities;

namespace FiapAgro.Domain.Interfaces;

public interface INotificador
{
    Task NotificarAsync(Alerta alerta);
    Task NotificarLoteAsync(IEnumerable<Alerta> alertas);
}
