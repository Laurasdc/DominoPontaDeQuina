using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Domain.Repositories;

public interface IJogadorRepository : IRepository<Jogador>
{
    Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<Jogador?> ObterComParticipacoesAsync(Guid id);
    Task<List<Jogador>> BuscarPorNomeAsync(string trecho);
}
