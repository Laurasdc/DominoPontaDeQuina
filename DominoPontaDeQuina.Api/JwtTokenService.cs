using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DominoPontaDeQuina.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace DominoPontaDeQuina.Api;

/// <summary>Emite tokens de curta duração para contas autenticadas.</summary>
public sealed class JwtTokenService(string issuer, string audience, string key)
{
    private readonly SigningCredentials credentials = new(
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

    /// <summary>Gera um token assinado com o identificador e o e-mail da conta.</summary>
    public TokenResponse Gerar(Usuario usuario)
    {
        var expiresAt = DateTime.UtcNow.AddHours(1);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims:
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ],
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}

/// <summary>Token JWT e instante de expiração em UTC.</summary>
public sealed record TokenResponse(string AccessToken, DateTime ExpiresAt);
