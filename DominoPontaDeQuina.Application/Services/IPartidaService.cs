using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Application.Services;

public interface IPartidaService : IService<Partida>
{
    Task<Partida?> ObterComParticipantesAsync(Guid id);
    Task<List<Partida>> ListarPorStatusAsync(StatusJogo status);
    Task<List<Partida>> ListarPorJogadorAsync(Guid jogadorId);
    Task IniciarAsync(Guid id);
    Task FinalizarAsync(Guid id);
}
