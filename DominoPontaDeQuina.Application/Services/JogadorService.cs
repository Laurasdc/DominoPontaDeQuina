using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;

namespace DominoPontaDeQuina.Application.Services;

public class JogadorService : ServiceBase<Jogador>, IJogadorService
{
    private readonly IJogadorRepository _jogadores;
    private readonly IUsuarioRepository _usuarios;
    public JogadorService(IJogadorRepository jogadores, IUsuarioRepository usuarios) : base(jogadores)
    {
        _jogadores = jogadores;
        _usuarios = usuarios;
    }

    protected override async Task ValidarAsync(Jogador jogador)
    {
        ArgumentNullException.ThrowIfNull(jogador);
        jogador.NomeExibicao = jogador.NomeExibicao?.Trim() ?? string.Empty;
        await base.ValidarAsync(jogador);
        if (await _usuarios.ObterPorIdAsync(jogador.UsuarioId) is null)
            throw new InvalidOperationException("Usuario nao encontrado.");
    }

    public Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId) => _jogadores.ListarPorUsuarioAsync(usuarioId);
    public Task<Jogador?> ObterComParticipacoesAsync(Guid id) => _jogadores.ObterComParticipacoesAsync(id);
    public Task<List<Jogador>> BuscarPorNomeAsync(string trecho) => _jogadores.BuscarPorNomeAsync(trecho);
}
