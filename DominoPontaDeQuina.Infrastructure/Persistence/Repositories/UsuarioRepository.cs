using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Infrastructure.Persistence.Repositories;

/// <summary>Implementa as consultas de contas no EF Core.</summary>
public sealed class UsuarioRepository(DominoDbContext db) : IUsuarioRepository
{
    /// <inheritdoc />
    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default) =>
        db.Usuarios.FirstOrDefaultAsync(usuario => usuario.Email == email, cancellationToken);

    /// <inheritdoc />
    public async Task<Usuario> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        db.Usuarios.Add(usuario);
        await db.SaveChangesAsync(cancellationToken);
        return usuario;
    }
}
