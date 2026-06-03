using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapAgro.Infrastructure.Repositories;

public class PropriedadeRepositoryEF : IPropriedadeRepository
{
    private readonly AppDbContext _db;

    public PropriedadeRepositoryEF(AppDbContext db) => _db = db;

    public async Task<Propriedade?> BuscarPorIdAsync(Guid id) =>
        await _db.Propriedades.FindAsync(id);

    public async Task<IEnumerable<Propriedade>> ListarPorUsuarioAsync(Guid usuarioId) =>
        await _db.Propriedades
            .Where(p => p.UsuarioId == usuarioId)
            .OrderBy(p => p.Nome)
            .ToListAsync();

    public async Task AdicionarAsync(Propriedade propriedade)
    {
        _db.Propriedades.Add(propriedade);
        await _db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Propriedade propriedade)
    {
        _db.Propriedades.Update(propriedade);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var propriedade = await _db.Propriedades.FindAsync(id);
        if (propriedade is not null)
        {
            _db.Propriedades.Remove(propriedade);
            await _db.SaveChangesAsync();
        }
    }
}
