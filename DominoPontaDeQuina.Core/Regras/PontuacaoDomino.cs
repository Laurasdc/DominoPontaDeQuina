namespace DominoPontaDeQuina.Core.Regras;

/// <summary>Calcula os pontos gerados pela soma das pontas externas.</summary>
public static class PontuacaoDomino
{
    /// <summary>Múltiplos de cinco geram soma / 5 pontos; outras somas geram zero.</summary>
    /// <exception cref="ArgumentOutOfRangeException">A soma é negativa.</exception>
    public static int Calcular(int soma)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(soma);
        return soma % 5 == 0 ? soma / 5 : 0;
    }
}
