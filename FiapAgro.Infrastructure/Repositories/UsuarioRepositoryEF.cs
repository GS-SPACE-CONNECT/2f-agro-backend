using FiapAgro.Domain.Entities;
using FiapAgro.Domain.Interfaces;
using FiapAgro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiapAgro.Infrastructure.Repositories;

public class UsuarioRepositoryEF : IUsuarioRepository
{
    private readonly AppDbContext _db;

    public UsuarioRepositoryEF(AppDbContext db) => _db = db;

    public async Task<Usuario?> BuscarPorEmailAsync(string email) =>
        await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant());

    public async Task<bool> ExisteEmailAsync(string email) =>
        await _db.Usuarios.AnyAsync(u => u.Email == email.ToLowerInvariant());

    public async Task AdicionarAsync(Usuario usuario)
    {
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
    }
}
