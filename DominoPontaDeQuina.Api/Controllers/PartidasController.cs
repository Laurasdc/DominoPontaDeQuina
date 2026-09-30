using DominoPontaDeQuina.Api.Contracts;
using DominoPontaDeQuina.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DominoPontaDeQuina.Api.Controllers;

[ApiController]
[Route("api/partidas")]
public sealed class PartidasController(IPartidaService partidas) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PartidaResponse>> Iniciar(IniciarPartidaRequest request,
        CancellationToken cancellationToken)
    {
        var partida = await partidas.IniciarPartidaAsync(request.PontuacaoAlvo, cancellationToken);
        return CreatedAtAction(nameof(Status), new { partidaId = partida.Id }, PartidaResponse.From(partida));
    }

    [HttpGet("{partidaId:guid}/status")]
    public async Task<ActionResult<PartidaResponse>> Status(Guid partidaId, CancellationToken cancellationToken)
    {
        var partida = await partidas.VerificarStatusAsync(partidaId, cancellationToken);
        return Ok(PartidaResponse.From(partida));
    }

    [HttpPost("{partidaId:guid}/jogadores")]
    public async Task<ActionResult<JogadorResponse>> RegistrarJogador(Guid partidaId,
        RegistrarJogadorRequest request, CancellationToken cancellationToken)
    {
        var jogador = await partidas.RegistrarJogadorAsync(partidaId, request.Nome.Trim(),
            request.UsuarioId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new JogadorResponse(jogador.Id, jogador.Nome));
    }

    [HttpPost("{partidaId:guid}/lances")]
    public async Task<ActionResult<LanceResponse>> RegistrarLance(Guid partidaId,
        RegistrarLanceRequest request, CancellationToken cancellationToken)
    {
        if (request.JogadorId == Guid.Empty)
            return BadRequest(new ProblemDetails { Status = 400, Title = "Informe o identificador do jogador." });
        var lance = await partidas.RegistrarLanceAsync(partidaId, request.JogadorId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created,
            new LanceResponse(lance.Id, partidaId, request.JogadorId, lance.Timestamp));
    }

    [HttpGet("historico")]
    public async Task<ActionResult<IReadOnlyList<PartidaResponse>>> Historico(CancellationToken cancellationToken)
    {
        var historico = await partidas.ConsultarHistoricoAsync(cancellationToken);
        return Ok(historico.Select(PartidaResponse.From).ToList());
    }

    [HttpGet("/api/ranking")]
    public async Task<ActionResult<IReadOnlyList<RankingResponse>>> Ranking(CancellationToken cancellationToken)
    {
        var ranking = await partidas.ConsultarRankingAsync(cancellationToken);
        return Ok(ranking.Select(item => new RankingResponse(item.Jogador.Id,
            item.Jogador.Nome, item.Vitorias)).ToList());
    }
}
