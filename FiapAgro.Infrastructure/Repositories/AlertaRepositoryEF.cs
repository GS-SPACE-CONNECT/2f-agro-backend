using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapAgro.Infrastructure.Repositories;

public class AlertaRepositoryEF : IAlertaRepository
{
    private readonly AppDbContext _db;

    public AlertaRepositoryEF(AppDbContext db) => _db = db;

    public async Task<Alerta?> BuscarPorIdAsync(Guid id) =>
        await _db.Alertas.FindAsync(id);

    public async Task<IEnumerable<Alerta>> ListarPorPropriedadeAsync(Guid propriedadeId) =>
        await _db.Alertas
            .Where(a => a.PropriedadeId == propriedadeId)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();

    public async Task AdicionarAsync(Alerta alerta)
    {
        _db.Alertas.Add(alerta);
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<Alerta>> ListarRecentesAsync(int quantidade = 20) =>
        await _db.Alertas
            .OrderByDescending(a => a.CriadoEm)
            .Take(quantidade)
            .ToListAsync();

    public async Task<IEnumerable<Alerta>> ListarPorPeriodoAsync(
        Guid propriedadeId, DateTime inicio, DateTime fim) =>
        await _db.Alertas
            .Where(a => a.PropriedadeId == propriedadeId
                        && a.CriadoEm >= inicio
                        && a.CriadoEm <= fim)
            .OrderByDescending(a => a.CriadoEm)
            .ToListAsync();
}
