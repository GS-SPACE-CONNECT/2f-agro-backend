using FiapAgro.Domain.Entities;

namespace FiapAgro.Domain.Interfaces;

public interface IDiagnosticoRepository
{
    Task<DiagnosticoPraga?> BuscarPorIdAsync(Guid id);
    Task<IEnumerable<DiagnosticoPraga>> ListarPorLavouraAsync(Guid lavouraId);
    Task<IEnumerable<DiagnosticoPraga>> ListarRecentesAsync(int quantidade = 10);
    Task AdicionarAsync(DiagnosticoPraga diagnostico);
}
