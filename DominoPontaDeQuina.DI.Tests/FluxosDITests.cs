using DominoPontaDeQuina.Application.Services;
using DominoPontaDeQuina.ConsoleApp;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DominoPontaDeQuina.DI.Tests;

public class FluxosDITests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly ServiceProvider _provider;
    private readonly IServiceScope _scope;
    private IServiceProvider Services => _scope.ServiceProvider;

    public FluxosDITests()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        var services = new ServiceCollection();
        services.AddDbContext<DominoDbContext>(o => o.UseSqlite(_connection));
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IJogadorRepository, JogadorRepository>();
        services.AddScoped<IPartidaRepository, PartidaRepository>();
        services.AddScoped<IParticipacaoPartidaRepository, ParticipacaoPartidaRepository>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IJogadorService, JogadorService>();
        services.AddScoped<IPartidaService, PartidaService>();
        services.AddScoped<IParticipacaoPartidaService, ParticipacaoPartidaService>();
        services.AddSingleton<TextWriter>(new StringWriter());
        services.AddScoped<FluxoPrincipal>();
        _provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true, ValidateScopes = true
        });
        _scope = _provider.CreateScope();
        Services.GetRequiredService<DominoDbContext>().Database.EnsureCreated();
    }

    [Fact]
    public async Task FluxoPrincipalExecutaCrudEConsultasComDependenciasInjetadas()
    {
        await Services.GetRequiredService<FluxoPrincipal>().ExecutarDemonstracaoAsync();
        Assert.Empty(await Services.GetRequiredService<IUsuarioService>().ListarAsync());
        Assert.Empty(await Services.GetRequiredService<IPartidaService>().ListarAsync());
        Assert.Empty(await Services.GetRequiredService<IJogadorService>().ListarAsync());
        Assert.Empty(await Services.GetRequiredService<IParticipacaoPartidaService>().ListarAsync());
    }

    [Fact]
    public async Task CadastroNormalizaEmailERecusaDuplicado()
    {
        var usuarios = Services.GetRequiredService<IUsuarioService>();
        var cadastrado = await usuarios.AdicionarAsync(new Usuario
        {
            Nome = "Laura", Email = " LAURA@example.com ", HashSenha = "hash-de-teste"
        });
        Assert.Equal("laura@example.com", cadastrado.Email);
        Assert.NotNull(await usuarios.ObterPorEmailAsync("LAURA@example.com"));
        await Assert.ThrowsAsync<InvalidOperationException>(() => usuarios.AdicionarAsync(new Usuario
        {
            Nome = "Outra", Email = "laura@example.com", HashSenha = "hash-de-teste"
        }));
    }

    [Fact]
    public async Task JogadorSemUsuarioNaoPodeSerCadastrado()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Services.GetRequiredService<IJogadorService>().AdicionarAsync(new Jogador
            {
                NomeExibicao = "Jogador", UsuarioId = Guid.NewGuid()
            }));
    }

    [Fact]
    public async Task ParticipacaoDuplicadaERejeitadaAntesDeSalvar()
    {
        var usuario = await Services.GetRequiredService<IUsuarioService>().AdicionarAsync(new Usuario
        {
            Nome = "Laura", Email = "laura@example.com", HashSenha = "hash-de-teste"
        });
        var jogador = await Services.GetRequiredService<IJogadorService>().AdicionarAsync(new Jogador
        {
            UsuarioId = usuario.Id, NomeExibicao = "Laura"
        });
        var partida = await Services.GetRequiredService<IPartidaService>().AdicionarAsync(new Partida());
        var participacoes = Services.GetRequiredService<IParticipacaoPartidaService>();
        await participacoes.AdicionarAsync(new ParticipacaoPartida
        {
            PartidaId = partida.Id, JogadorId = jogador.Id
        });
        await Assert.ThrowsAsync<InvalidOperationException>(() => participacoes.AdicionarAsync(new ParticipacaoPartida
        {
            PartidaId = partida.Id, JogadorId = jogador.Id
        }));
        Assert.Single(await participacoes.ListarPorPartidaAsync(partida.Id));
    }

    [Fact]
    public async Task PartidaAguardandoNaoPodeSerFinalizada()
    {
        var partidas = Services.GetRequiredService<IPartidaService>();
        var partida = await partidas.AdicionarAsync(new Partida());
        await Assert.ThrowsAsync<InvalidOperationException>(() => partidas.FinalizarAsync(partida.Id));
        Assert.Equal(StatusJogo.Aguardando, (await partidas.ObterPorIdAsync(partida.Id))!.Status);
    }

    public void Dispose()
    {
        _scope.Dispose();
        _provider.Dispose();
        _connection.Dispose();
    }
}
