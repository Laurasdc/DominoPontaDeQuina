using DominoPontaDeQuina.Application.Services;
using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.ConsoleApp;

public class FluxoPrincipal
{
    private readonly IUsuarioService _usuarios;
    private readonly IJogadorService _jogadores;
    private readonly IPartidaService _partidas;
    private readonly IParticipacaoPartidaService _participacoes;
    private readonly TextWriter _saida;

    public FluxoPrincipal(IUsuarioService usuarios, IJogadorService jogadores,
        IPartidaService partidas, IParticipacaoPartidaService participacoes, TextWriter saida)
    {
        _usuarios = usuarios;
        _jogadores = jogadores;
        _partidas = partidas;
        _participacoes = participacoes;
        _saida = saida;
    }

    public async Task ListarAsync()
    {
        var usuarios = await _usuarios.ListarAsync();
        var partidas = await _partidas.ListarAsync();
        await _saida.WriteLineAsync($"Usuarios cadastrados: {usuarios.Count}; partidas: {partidas.Count}");
        foreach (var usuario in usuarios)
            await _saida.WriteLineAsync($"{usuario.Nome} - {usuario.Email}");
    }

    public async Task ExecutarDemonstracaoAsync()
    {
        var usuario = await _usuarios.AdicionarAsync(new Usuario
        {
            Nome = "Usuario DI", Email = "di@example.com", HashSenha = "hash-exclusivo-de-teste"
        });
        var jogador = await _jogadores.AdicionarAsync(new Jogador
        {
            UsuarioId = usuario.Id, NomeExibicao = "Jogador DI"
        });
        var partida = await _partidas.AdicionarAsync(new Partida());
        var participacao = await _participacoes.AdicionarAsync(new ParticipacaoPartida
        {
            PartidaId = partida.Id, JogadorId = jogador.Id, Posicao = 0
        });
        await _partidas.IniciarAsync(partida.Id);
        participacao.Pontuacao = 10;
        participacao.Vencedor = true;
        await _participacoes.AtualizarAsync(participacao);
        await _partidas.FinalizarAsync(partida.Id);

        var porUsuario = await _jogadores.ListarPorUsuarioAsync(usuario.Id);
        var porJogador = await _partidas.ListarPorJogadorAsync(jogador.Id);
        var vencedores = await _participacoes.ListarVencedoresAsync();
        var finalizada = await _partidas.ObterComParticipantesAsync(partida.Id);
        if (porUsuario.Count != 1 || porJogador.Count != 1 || vencedores.Count != 1
            || finalizada?.Status != StatusJogo.Finalizado || finalizada.Participacoes.Count != 1)
            throw new InvalidOperationException("Falha nas consultas do fluxo DI.");

        if (!await _participacoes.RemoverAsync(participacao.Id)
            || !await _partidas.RemoverAsync(partida.Id)
            || !await _jogadores.RemoverAsync(jogador.Id)
            || !await _usuarios.RemoverAsync(usuario.Id))
            throw new InvalidOperationException("Falha na remocao dos dados de demonstracao.");
        await _saida.WriteLineAsync("DI OK: cadastro, consultas LINQ, atualizacao, finalizacao e remocao.");
    }
}
