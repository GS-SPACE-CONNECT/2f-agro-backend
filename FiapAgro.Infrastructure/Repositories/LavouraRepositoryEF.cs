using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapAgro.Infrastructure.Repositories;

public class LavouraRepositoryEF : ILavouraRepository
{
    private readonly AppDbContext _db;

    public LavouraRepositoryEF(AppDbContext db) => _db = db;

    public async Task<Lavoura?> BuscarPorIdAsync(Guid id) =>
        await _db.Lavouras.FindAsync(id);

    public async Task<IEnumerable<Lavoura>> ListarPorPropriedadeAsync(Guid propriedadeId) =>
        await _db.Lavouras
            .Where(l => l.PropriedadeId == propriedadeId)
            .OrderBy(l => l.Identificador)
            .ToListAsync();

    public async Task AdicionarAsync(Lavoura lavoura)
    {
        _db.Lavouras.Add(lavoura);
        await _db.SaveChangesAsync();
    }

    public async Task AtualizarAsync(Lavoura lavoura)
    {
        _db.Lavouras.Update(lavoura);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var lavoura = await _db.Lavouras.FindAsync(id);
        if (lavoura is not null)
        {
            _db.Lavouras.Remove(lavoura);
            await _db.SaveChangesAsync();
        }
    }
}
