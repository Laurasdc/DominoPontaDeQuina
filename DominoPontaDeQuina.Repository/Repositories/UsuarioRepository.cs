using DominoPontaDeQuina.Domain.Repositories;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(DominoDbContext context) : base(context) { }

    public Task<Usuario?> ObterPorEmailAsync(string email) =>
        Entidades.AsNoTracking().FirstOrDefaultAsync(usuario => usuario.Email == email);

    public Task<Usuario?> ObterComJogadoresAsync(Guid id) =>
        Entidades.AsNoTracking().Include(usuario => usuario.Jogadores)
            .FirstOrDefaultAsync(usuario => usuario.Id == id);

    public Task<List<Usuario>> BuscarPorNomeAsync(string trecho) =>
        Entidades.AsNoTracking().Where(usuario => usuario.Nome.Contains(trecho))
            .OrderBy(usuario => usuario.Nome).ToListAsync();

    public Task<bool> EmailEmUsoAsync(string email) =>
        Entidades.AnyAsync(usuario => usuario.Email == email);
}
