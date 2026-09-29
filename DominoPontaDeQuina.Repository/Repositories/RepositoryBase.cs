using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public abstract class RepositoryBase<TEntity> where TEntity : class
{
    protected readonly DominoDbContext Context;
    protected DbSet<TEntity> Entidades => Context.Set<TEntity>();

    protected RepositoryBase(DominoDbContext context) => Context = context;

    public async Task<TEntity> AdicionarAsync(TEntity entidade)
    {
        await Entidades.AddAsync(entidade);
        await Context.SaveChangesAsync();
        return entidade;
    }

    public async Task<TEntity?> ObterPorIdAsync(Guid id) => await Entidades.FindAsync(id);

    public Task<List<TEntity>> ListarAsync() => Entidades.AsNoTracking().ToListAsync();

    public async Task AtualizarAsync(TEntity entidade)
    {
        Entidades.Update(entidade);
        await Context.SaveChangesAsync();
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var entidade = await Entidades.FindAsync(id);
        if (entidade is null)
            return false;

        Entidades.Remove(entidade);
        await Context.SaveChangesAsync();
        return true;
    }
}
