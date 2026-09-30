using System.ComponentModel.DataAnnotations;
using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Api.Contracts;

public sealed class IniciarPartidaRequest
{
    [Range(1, int.MaxValue)]
    public int PontuacaoAlvo { get; set; } = 50;
}

public sealed class RegistrarJogadorRequest
{
    [Required, StringLength(150)]
    public string Nome { get; set; } = string.Empty;
    public Guid? UsuarioId { get; set; }
}

public sealed class RegistrarLanceRequest
{
    public Guid JogadorId { get; set; }
}

public sealed record PartidaResponse(Guid Id, int PontuacaoAlvo, string Status, DateTime CriadaEm)
{
    public static PartidaResponse From(Partida partida) =>
        new(partida.Id, partida.PontuacaoAlvo, partida.Status, partida.CriadaEm);
}

public sealed record JogadorResponse(Guid Id, string Nome);
public sealed record LanceResponse(Guid Id, Guid PartidaId, Guid JogadorId, DateTime Timestamp);
public sealed record RankingResponse(Guid JogadorId, string Nome, int Vitorias);
