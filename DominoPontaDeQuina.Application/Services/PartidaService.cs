using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;

namespace DominoPontaDeQuina.Application.Services;

public class PartidaService : ServiceBase<Partida>, IPartidaService
{
    private readonly IPartidaRepository _partidas;
    public PartidaService(IPartidaRepository partidas) : base(partidas) => _partidas = partidas;

    protected override async Task ValidarAsync(Partida partida)
    {
        await base.ValidarAsync(partida);
        if (!Enum.IsDefined(partida.Status))
            throw new ArgumentException("Status de partida invalido.");
        if (partida.FinalizadoEm < partida.IniciadoEm)
            throw new ArgumentException("A finalizacao deve ocorrer depois do inicio.");
    }

    public async Task IniciarAsync(Guid id)
    {
        var partida = await _partidas.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Partida nao encontrada.");
        if (partida.Status != StatusJogo.Aguardando)
            throw new InvalidOperationException("Somente uma partida aguardando pode ser iniciada.");
        partida.Status = StatusJogo.EmAndamento;
        partida.IniciadoEm = DateTime.UtcNow;
        await AtualizarAsync(partida);
    }

    public async Task FinalizarAsync(Guid id)
    {
        var partida = await _partidas.ObterPorIdAsync(id)
            ?? throw new KeyNotFoundException("Partida nao encontrada.");
        if (partida.Status != StatusJogo.EmAndamento)
            throw new InvalidOperationException("Somente uma partida em andamento pode ser finalizada.");
        partida.Status = StatusJogo.Finalizado;
        partida.FinalizadoEm = DateTime.UtcNow;
        await AtualizarAsync(partida);
    }

    public Task<Partida?> ObterComParticipantesAsync(Guid id) => _partidas.ObterComParticipantesAsync(id);
    public Task<List<Partida>> ListarPorStatusAsync(StatusJogo status) => _partidas.ListarPorStatusAsync(status);
    public Task<List<Partida>> ListarPorJogadorAsync(Guid jogadorId) => _partidas.ListarPorJogadorAsync(jogadorId);
}
