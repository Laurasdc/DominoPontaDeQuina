using DominoPontaDeQuina.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Context;

public class DominoDbContext : DbContext
{
    public DominoDbContext(DbContextOptions<DominoDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Jogador> Jogadores => Set<Jogador>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<ParticipacaoPartida> ParticipacoesPartida => Set<ParticipacaoPartida>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasMany(usuario => usuario.Jogadores)
            .WithOne(jogador => jogador.Usuario)
            .HasForeignKey(jogador => jogador.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Jogador>()
            .HasMany(jogador => jogador.Participacoes)
            .WithOne(participacao => participacao.Jogador)
            .HasForeignKey(participacao => participacao.JogadorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Partida>()
            .HasMany(partida => partida.Participacoes)
            .WithOne(participacao => participacao.Partida)
            .HasForeignKey(participacao => participacao.PartidaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ParticipacaoPartida>()
            .HasIndex(participacao => new { participacao.PartidaId, participacao.JogadorId })
            .IsUnique();
    }
}
