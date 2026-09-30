namespace DominoPontaDeQuina.Application.Services;

public interface IService<TEntity> where TEntity : class
{
    Task<TEntity> AdicionarAsync(TEntity entidade);
    Task<TEntity?> ObterPorIdAsync(Guid id);
    Task<List<TEntity>> ListarAsync();
    Task AtualizarAsync(TEntity entidade);
    Task<bool> RemoverAsync(Guid id);
}
