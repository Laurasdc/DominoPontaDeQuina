using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Domain.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario?> ObterComJogadoresAsync(Guid id);
    Task<List<Usuario>> BuscarPorNomeAsync(string trecho);
    Task<bool> EmailEmUsoAsync(string email);
}
