using DominoPontaDeQuina.Application.Services;
using DominoPontaDeQuina.ConsoleApp;
using DominoPontaDeQuina.Domain.Repositories;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var demonstracao = args.Contains("--demo");
var services = new ServiceCollection();
services.AddDbContext<DominoDbContext>(options => options.UseSqlite(demonstracao
    ? "Data Source=:memory:"
    : Environment.GetEnvironmentVariable("DOMINO_CONNECTION") ?? "Data Source=domino.db"));
services.AddScoped<IUsuarioRepository, UsuarioRepository>();
services.AddScoped<IJogadorRepository, JogadorRepository>();
services.AddScoped<IPartidaRepository, PartidaRepository>();
services.AddScoped<IParticipacaoPartidaRepository, ParticipacaoPartidaRepository>();
services.AddScoped<IUsuarioService, UsuarioService>();
services.AddScoped<IJogadorService, JogadorService>();
services.AddScoped<IPartidaService, PartidaService>();
services.AddScoped<IParticipacaoPartidaService, ParticipacaoPartidaService>();
services.AddSingleton<TextWriter>(System.Console.Out);
services.AddScoped<FluxoPrincipal>();

using var provider = services.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateScopes = true,
    ValidateOnBuild = true
});
using var scope = provider.CreateScope();
var fluxo = scope.ServiceProvider.GetRequiredService<FluxoPrincipal>();
if (demonstracao)
{
    var db = scope.ServiceProvider.GetRequiredService<DominoDbContext>();
    await db.Database.OpenConnectionAsync();
    await db.Database.EnsureCreatedAsync();
    await fluxo.ExecutarDemonstracaoAsync();
}
else
{
    await fluxo.ListarAsync();
}
