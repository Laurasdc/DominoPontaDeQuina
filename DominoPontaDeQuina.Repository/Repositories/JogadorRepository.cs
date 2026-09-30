using DominoPontaDeQuina.Domain.Repositories;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class JogadorRepository : RepositoryBase<Jogador>, IJogadorRepository
{
    public JogadorRepository(DominoDbContext context) : base(context) { }

    public Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId) =>
        Entidades.AsNoTracking().Where(jogador => jogador.UsuarioId == usuarioId)
            .OrderBy(jogador => jogador.NomeExibicao).ToListAsync();

    public Task<Jogador?> ObterComParticipacoesAsync(Guid id) =>
        Entidades.AsNoTracking().Include(jogador => jogador.Participacoes)
            .FirstOrDefaultAsync(jogador => jogador.Id == id);

    public Task<List<Jogador>> BuscarPorNomeAsync(string trecho) =>
        Entidades.AsNoTracking().Where(jogador => jogador.NomeExibicao.Contains(trecho))
            .OrderBy(jogador => jogador.NomeExibicao).ToListAsync();
}
