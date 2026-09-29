using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class PartidaRepository : RepositoryBase<Partida>
{
    public PartidaRepository(DominoDbContext context) : base(context) { }

    public Task<Partida?> ObterComParticipantesAsync(Guid id) =>
        Entidades.AsNoTracking().Include(partida => partida.Participacoes)
            .ThenInclude(participacao => participacao.Jogador)
            .FirstOrDefaultAsync(partida => partida.Id == id);

    public Task<List<Partida>> ListarPorStatusAsync(StatusJogo status) =>
        Entidades.AsNoTracking().Where(partida => partida.Status == status)
            .OrderByDescending(partida => partida.IniciadoEm).ToListAsync();

    public Task<List<Partida>> ListarPorJogadorAsync(Guid jogadorId) =>
        Entidades.AsNoTracking()
            .Where(partida => partida.Participacoes.Any(participacao => participacao.JogadorId == jogadorId))
            .OrderByDescending(partida => partida.IniciadoEm).ToListAsync();
}
