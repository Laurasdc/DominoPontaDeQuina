using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Domain.Repositories;

public interface IParticipacaoPartidaRepository : IRepository<ParticipacaoPartida>
{
    Task<List<ParticipacaoPartida>> ListarPorPartidaAsync(Guid partidaId);
    Task<List<ParticipacaoPartida>> ListarPorJogadorAsync(Guid jogadorId);
    Task<List<ParticipacaoPartida>> ListarVencedoresAsync();
}
