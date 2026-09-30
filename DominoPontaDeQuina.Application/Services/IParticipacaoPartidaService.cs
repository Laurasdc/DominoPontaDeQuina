using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Application.Services;

public interface IParticipacaoPartidaService : IService<ParticipacaoPartida>
{
    Task<List<ParticipacaoPartida>> ListarPorPartidaAsync(Guid partidaId);
    Task<List<ParticipacaoPartida>> ListarPorJogadorAsync(Guid jogadorId);
    Task<List<ParticipacaoPartida>> ListarVencedoresAsync();
}
