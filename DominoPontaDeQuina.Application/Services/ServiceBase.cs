using System.ComponentModel.DataAnnotations;
using DominoPontaDeQuina.Domain.Repositories;

namespace DominoPontaDeQuina.Application.Services;

public abstract class ServiceBase<TEntity> : IService<TEntity> where TEntity : class
{
    private readonly IRepository<TEntity> _repository;

    protected ServiceBase(IRepository<TEntity> repository) => _repository = repository;

    protected virtual Task ValidarAsync(TEntity entidade)
    {
        ArgumentNullException.ThrowIfNull(entidade);
        Validator.ValidateObject(entidade, new ValidationContext(entidade), validateAllProperties: true);
        return Task.CompletedTask;
    }

    public async Task<TEntity> AdicionarAsync(TEntity entidade)
    {
        await ValidarAsync(entidade);
        return await _repository.AdicionarAsync(entidade);
    }

    public Task<TEntity?> ObterPorIdAsync(Guid id) => _repository.ObterPorIdAsync(id);
    public Task<List<TEntity>> ListarAsync() => _repository.ListarAsync();

    public async Task AtualizarAsync(TEntity entidade)
    {
        await ValidarAsync(entidade);
        await _repository.AtualizarAsync(entidade);
    }

    public Task<bool> RemoverAsync(Guid id) => _repository.RemoverAsync(id);
}
