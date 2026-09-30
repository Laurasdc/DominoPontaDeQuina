namespace DominoPontaDeQuina.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<TEntity> AdicionarAsync(TEntity entidade);
    Task<TEntity?> ObterPorIdAsync(Guid id);
    Task<List<TEntity>> ListarAsync();
    Task AtualizarAsync(TEntity entidade);
    Task<bool> RemoverAsync(Guid id);
}
