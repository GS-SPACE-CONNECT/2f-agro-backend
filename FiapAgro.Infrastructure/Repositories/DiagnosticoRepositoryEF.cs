using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapAgro.Infrastructure.Repositories;

public class DiagnosticoRepositoryEF : IDiagnosticoRepository
{
    private readonly AppDbContext _db;

    public DiagnosticoRepositoryEF(AppDbContext db) => _db = db;

    public async Task<DiagnosticoPraga?> BuscarPorIdAsync(Guid id) =>
        await _db.Diagnosticos.FindAsync(id);

    public async Task<IEnumerable<DiagnosticoPraga>> ListarPorLavouraAsync(Guid lavouraId) =>
        await _db.Diagnosticos
            .Where(d => d.LavouraId == lavouraId)
            .OrderByDescending(d => d.CriadoEm)
            .ToListAsync();

    public async Task<IEnumerable<DiagnosticoPraga>> ListarRecentesAsync(int quantidade = 10) =>
        await _db.Diagnosticos
            .OrderByDescending(d => d.CriadoEm)
            .Take(quantidade)
            .ToListAsync();

    public async Task AdicionarAsync(DiagnosticoPraga diagnostico)
    {
        _db.Diagnosticos.Add(diagnostico);
        await _db.SaveChangesAsync();
    }
}
