using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class ParticipacaoPartidaRepository : RepositoryBase<ParticipacaoPartida>
{
    public ParticipacaoPartidaRepository(DominoDbContext context) : base(context) { }

    public Task<List<ParticipacaoPartida>> ListarPorPartidaAsync(Guid partidaId) =>
        Entidades.AsNoTracking().Include(participacao => participacao.Jogador)
            .Where(participacao => participacao.PartidaId == partidaId)
            .OrderBy(participacao => participacao.Posicao).ToListAsync();

    public Task<List<ParticipacaoPartida>> ListarPorJogadorAsync(Guid jogadorId) =>
        Entidades.AsNoTracking().Include(participacao => participacao.Partida)
            .Where(participacao => participacao.JogadorId == jogadorId)
            .OrderByDescending(participacao => participacao.Partida.IniciadoEm).ToListAsync();

    public Task<List<ParticipacaoPartida>> ListarVencedoresAsync() =>
        Entidades.AsNoTracking().Where(participacao => participacao.Vencedor)
            .OrderByDescending(participacao => participacao.Pontuacao).ToListAsync();
}
