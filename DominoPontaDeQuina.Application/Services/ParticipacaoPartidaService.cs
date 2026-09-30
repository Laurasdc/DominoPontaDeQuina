using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;

namespace DominoPontaDeQuina.Application.Services;

public class ParticipacaoPartidaService : ServiceBase<ParticipacaoPartida>, IParticipacaoPartidaService
{
    private readonly IParticipacaoPartidaRepository _participacoes;
    private readonly IPartidaRepository _partidas;
    private readonly IJogadorRepository _jogadores;

    public ParticipacaoPartidaService(IParticipacaoPartidaRepository participacoes,
        IPartidaRepository partidas, IJogadorRepository jogadores) : base(participacoes)
    {
        _participacoes = participacoes;
        _partidas = partidas;
        _jogadores = jogadores;
    }

    protected override async Task ValidarAsync(ParticipacaoPartida participacao)
    {
        await base.ValidarAsync(participacao);
        if (participacao.Posicao < 0 || participacao.Pontuacao < 0)
            throw new ArgumentException("Posicao e pontuacao nao podem ser negativas.");
        if (await _partidas.ObterPorIdAsync(participacao.PartidaId) is null)
            throw new InvalidOperationException("Partida nao encontrada.");
        if (await _jogadores.ObterPorIdAsync(participacao.JogadorId) is null)
            throw new InvalidOperationException("Jogador nao encontrado.");
        var existentes = await _participacoes.ListarPorPartidaAsync(participacao.PartidaId);
        if (existentes.Exists(p => p.Id != participacao.Id && p.JogadorId == participacao.JogadorId))
            throw new InvalidOperationException("O jogador ja participa desta partida.");
    }

    public Task<List<ParticipacaoPartida>> ListarPorPartidaAsync(Guid partidaId) => _participacoes.ListarPorPartidaAsync(partidaId);
    public Task<List<ParticipacaoPartida>> ListarPorJogadorAsync(Guid jogadorId) => _participacoes.ListarPorJogadorAsync(jogadorId);
    public Task<List<ParticipacaoPartida>> ListarVencedoresAsync() => _participacoes.ListarVencedoresAsync();
}
