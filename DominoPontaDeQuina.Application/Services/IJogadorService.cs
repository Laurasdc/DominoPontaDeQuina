using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Application.Services;

public interface IJogadorService : IService<Jogador>
{
    Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<Jogador?> ObterComParticipacoesAsync(Guid id);
    Task<List<Jogador>> BuscarPorNomeAsync(string trecho);
}
