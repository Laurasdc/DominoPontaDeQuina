using DominoPontaDeQuina.Core.Regras;
using FluentAssertions;
using Xunit;

namespace DominoPontaDeQuina.UnitTests;

public class PontuacaoDominoTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(5, 1)]
    [InlineData(10, 2)]
    [InlineData(15, 3)]
    [InlineData(20, 4)]
    [InlineData(1, 0)]
    [InlineData(9, 0)]
    [InlineData(12, 0)]
    public void Calcular_DeveRetornarPontosConformeSoma(int soma, int esperado)
    {
        // Arrange: entradas fornecidas pelo Theory.

        // Act
        var pontos = PontuacaoDomino.Calcular(soma);

        // Assert
        pontos.Should().Be(esperado);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    public void Calcular_ComSomaNegativa_DeveLancarExcecao(int soma)
    {
        // Arrange
        Action calcular = () => PontuacaoDomino.Calcular(soma);

        // Act & Assert
        calcular.Should().Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be("soma");
    }
}
