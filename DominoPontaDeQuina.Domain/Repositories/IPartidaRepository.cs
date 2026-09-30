using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Domain.Repositories;

public interface IPartidaRepository : IRepository<Partida>
{
    Task<Partida?> ObterComParticipantesAsync(Guid id);
    Task<List<Partida>> ListarPorStatusAsync(StatusJogo status);
    Task<List<Partida>> ListarPorJogadorAsync(Guid jogadorId);
}
