using System.Net;
using System.Net.Http.Json;
using DominoPontaDeQuina.Api.Contracts;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DominoPontaDeQuina.Api.Tests;

public sealed class EndpointsTests : IDisposable
{
    private readonly ApiFactory _factory = new();
    private readonly HttpClient _client;

    public EndpointsTests()
    {
        _client = _factory.CreateClient();
        using var scope = _factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<DominoDbContext>().Database.EnsureCreated();
    }

    [Fact]
    public async Task IniciaPartidaRegistraJogadorELanceEConsultaStatusEHistorico()
    {
        var criada = await _client.PostAsJsonAsync("/api/partidas", new { pontuacaoAlvo = 100 });
        Assert.Equal(HttpStatusCode.Created, criada.StatusCode);
        var partida = (await criada.Content.ReadFromJsonAsync<PartidaResponse>())!;
        Assert.Equal(100, partida.PontuacaoAlvo);
        Assert.Equal("EmAndamento", partida.Status);
        Assert.EndsWith($"/api/partidas/{partida.Id}/status", criada.Headers.Location!.ToString());

        var cadastro = await _client.PostAsJsonAsync($"/api/partidas/{partida.Id}/jogadores", new { nome = "Laura" });
        Assert.Equal(HttpStatusCode.Created, cadastro.StatusCode);
        var jogador = (await cadastro.Content.ReadFromJsonAsync<JogadorResponse>())!;
        var jogada = await _client.PostAsJsonAsync($"/api/partidas/{partida.Id}/lances", new { jogadorId = jogador.Id });
        Assert.Equal(HttpStatusCode.Created, jogada.StatusCode);
        var lance = (await jogada.Content.ReadFromJsonAsync<LanceResponse>())!;
        Assert.Equal(jogador.Id, lance.JogadorId);
        Assert.Equal(partida.Id, lance.PartidaId);
        var status = await _client.GetFromJsonAsync<PartidaResponse>($"/api/partidas/{partida.Id}/status");
        Assert.Equal(partida.Id, status!.Id);
        var historico = await _client.GetFromJsonAsync<List<PartidaResponse>>("/api/partidas/historico");
        Assert.Contains(historico!, item => item.Id == partida.Id);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DominoDbContext>();
        Assert.Equal(1, await db.Lances.CountAsync());
        Assert.Equal(1, await db.ParticipacoesPartida.CountAsync());
    }

    [Fact]
    public async Task PontuacaoAlvoInvalidaRetorna400()
    {
        var resposta = await _client.PostAsJsonAsync("/api/partidas", new { pontuacaoAlvo = 0 });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task NomeVazioRetorna400()
    {
        var resposta = await _client.PostAsJsonAsync($"/api/partidas/{Guid.NewGuid()}/jogadores", new { nome = " " });
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PartidaInexistenteRetorna404()
    {
        var resposta = await _client.GetAsync($"/api/partidas/{Guid.NewGuid()}/status");
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task RankingRetornaJogadoresOrdenadosSemCiclosDeNavegacao()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DominoDbContext>();
            db.Rankings.AddRange(
                new Ranking { Jogador = new Jogador { Nome = "Laura" }, Vitorias = 2 },
                new Ranking { Jogador = new Jogador { Nome = "Ana" }, Vitorias = 5 });
            await db.SaveChangesAsync();
        }
        var ranking = await _client.GetFromJsonAsync<List<RankingResponse>>("/api/ranking");
        Assert.Equal(2, ranking!.Count);
        Assert.Equal("Ana", ranking[0].Nome);
        Assert.Equal(5, ranking[0].Vitorias);
        Assert.NotEqual(Guid.Empty, ranking[0].JogadorId);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }
}
