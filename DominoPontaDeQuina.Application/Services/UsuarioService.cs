using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;

namespace DominoPontaDeQuina.Application.Services;

public class UsuarioService : ServiceBase<Usuario>, IUsuarioService
{
    private readonly IUsuarioRepository _usuarios;
    public UsuarioService(IUsuarioRepository usuarios) : base(usuarios) => _usuarios = usuarios;

    protected override async Task ValidarAsync(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        usuario.Nome = usuario.Nome?.Trim() ?? string.Empty;
        usuario.Email = usuario.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        await base.ValidarAsync(usuario);
        var existente = await _usuarios.ObterPorEmailAsync(usuario.Email);
        if (existente is not null && existente.Id != usuario.Id)
            throw new InvalidOperationException("O e-mail ja esta cadastrado.");
    }

    public Task<Usuario?> ObterPorEmailAsync(string email) => _usuarios.ObterPorEmailAsync(email.Trim().ToLowerInvariant());
    public Task<Usuario?> ObterComJogadoresAsync(Guid id) => _usuarios.ObterComJogadoresAsync(id);
    public Task<List<Usuario>> BuscarPorNomeAsync(string trecho) => _usuarios.BuscarPorNomeAsync(trecho);
    public Task<bool> EmailEmUsoAsync(string email) => _usuarios.EmailEmUsoAsync(email.Trim().ToLowerInvariant());
}
