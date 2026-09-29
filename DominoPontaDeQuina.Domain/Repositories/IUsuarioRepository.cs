using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Domain.Repositories;

/// <summary>Consulta e persiste contas usadas na autenticação.</summary>
public interface IUsuarioRepository
{
    /// <summary>Obtém uma conta pelo e-mail normalizado.</summary>
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Adiciona uma conta com a senha já transformada em hash.</summary>
    Task<Usuario> AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
}
