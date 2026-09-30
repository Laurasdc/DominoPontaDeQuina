using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Application.Services;

public interface IUsuarioService : IService<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterComJogadoresAsync(Guid id);
    Task<List<Usuario>> BuscarPorNomeAsync(string trecho);
    Task<bool> EmailEmUsoAsync(string email);
}
