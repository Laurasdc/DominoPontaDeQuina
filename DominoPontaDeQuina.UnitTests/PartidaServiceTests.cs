using DominoPontaDeQuina.Application.Services;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace DominoPontaDeQuina.UnitTests;

public class PartidaServiceTests
{
    private readonly Mock<IPartidaRepository> _partidas = new(MockBehavior.Strict);
    private readonly Mock<IJogadorRepository> _jogadores = new(MockBehavior.Strict);
    private readonly Mock<IParticipacaoRepository> _participacoes = new(MockBehavior.Strict);
    private readonly Mock<ILanceRepository> _lances = new(MockBehavior.Strict);
    private readonly Mock<IRankingRepository> _ranking = new(MockBehavior.Strict);

    private PartidaService CriarServico() => new(
        _partidas.Object, _jogadores.Object, _participacoes.Object,
        _lances.Object, _ranking.Object);

    [Fact]
    public async Task RegistrarLance_ComPartidaEmAndamentoEJogadorExistente_DeveConsultarESalvar()
    {
        // Arrange
        var partidaId = Guid.NewGuid();
        var jogadorId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var token = cancellation.Token;
        var partida = new Partida { Id = partidaId, Status = "EmAndamento" };
        Lance? salvo = null;
        var ordem = new MockSequence();
        _partidas.InSequence(ordem).Setup(r => r.ObterAsync(partidaId, token))
            .ReturnsAsync(partida);
        _jogadores.InSequence(ordem).Setup(r => r.ExisteAsync(jogadorId, token))
            .ReturnsAsync(true);
        _lances.InSequence(ordem).Setup(r => r.AdicionarNaPartidaAsync(
                It.IsAny<Lance>(), partidaId, jogadorId, token))
            .Callback<Lance, Guid, Guid, CancellationToken>((lance, _, _, _) => salvo = lance)
            .ReturnsAsync((Lance lance, Guid _, Guid _, CancellationToken _) => lance);
        var servico = CriarServico();
        var antes = DateTime.UtcNow;

        // Act
        var resultado = await servico.RegistrarLanceAsync(partidaId, jogadorId, token);

        // Assert
        resultado.Should().BeSameAs(salvo);
        resultado.Id.Should().NotBeEmpty();
        resultado.Timestamp.Should().BeOnOrAfter(antes).And.BeOnOrBefore(DateTime.UtcNow);
        _partidas.Verify(r => r.ObterAsync(partidaId, token), Times.Once);
        _jogadores.Verify(r => r.ExisteAsync(jogadorId, token), Times.Once);
        _lances.Verify(r => r.AdicionarNaPartidaAsync(It.IsAny<Lance>(), partidaId, jogadorId, token), Times.Once);
        VerificarAusenciaDeOutrasChamadas();
    }

    [Fact]
    public async Task RegistrarLance_ComPartidaInexistente_NaoDeveConsultarJogadorNemSalvar()
    {
        // Arrange
        var partidaId = Guid.NewGuid();
        var jogadorId = Guid.NewGuid();
        _partidas.Setup(r => r.ObterAsync(partidaId, CancellationToken.None))
            .ReturnsAsync((Partida?)null);
        var servico = CriarServico();
        Func<Task> registrar = () => servico.RegistrarLanceAsync(partidaId, jogadorId);

        // Act & Assert
        await registrar.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Partida não encontrada.");
        _partidas.Verify(r => r.ObterAsync(partidaId, CancellationToken.None), Times.Once);
        VerificarNenhumLanceSalvo();
        VerificarAusenciaDeOutrasChamadas();
    }

    [Fact]
    public async Task RegistrarLance_ComPartidaFinalizada_NaoDeveConsultarJogadorNemSalvar()
    {
        // Arrange
        var partidaId = Guid.NewGuid();
        _partidas.Setup(r => r.ObterAsync(partidaId, CancellationToken.None))
            .ReturnsAsync(new Partida { Id = partidaId, Status = "Finalizada" });
        var servico = CriarServico();
        Func<Task> registrar = () => servico.RegistrarLanceAsync(partidaId, Guid.NewGuid());

        // Act & Assert
        await registrar.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("A partida não está em andamento.");
        _partidas.Verify(r => r.ObterAsync(partidaId, CancellationToken.None), Times.Once);
        VerificarNenhumLanceSalvo();
        VerificarAusenciaDeOutrasChamadas();
    }

    [Fact]
    public async Task RegistrarLance_ComJogadorInexistente_DeveConsultarMasNaoSalvar()
    {
        // Arrange
        var partidaId = Guid.NewGuid();
        var jogadorId = Guid.NewGuid();
        _partidas.Setup(r => r.ObterAsync(partidaId, CancellationToken.None))
            .ReturnsAsync(new Partida { Id = partidaId, Status = "EmAndamento" });
        _jogadores.Setup(r => r.ExisteAsync(jogadorId, CancellationToken.None))
            .ReturnsAsync(false);
        var servico = CriarServico();
        Func<Task> registrar = () => servico.RegistrarLanceAsync(partidaId, jogadorId);

        // Act & Assert
        await registrar.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Jogador não encontrado.");
        _partidas.Verify(r => r.ObterAsync(partidaId, CancellationToken.None), Times.Once);
        _jogadores.Verify(r => r.ExisteAsync(jogadorId, CancellationToken.None), Times.Once);
        VerificarNenhumLanceSalvo();
        VerificarAusenciaDeOutrasChamadas();
    }

    private void VerificarNenhumLanceSalvo() =>
        _lances.Verify(r => r.AdicionarNaPartidaAsync(It.IsAny<Lance>(),
            It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);

    private void VerificarAusenciaDeOutrasChamadas()
    {
        _partidas.VerifyNoOtherCalls();
        _jogadores.VerifyNoOtherCalls();
        _participacoes.VerifyNoOtherCalls();
        _lances.VerifyNoOtherCalls();
        _ranking.VerifyNoOtherCalls();
    }
}
