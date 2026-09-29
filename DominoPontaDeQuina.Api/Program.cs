using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using DominoPontaDeQuina.Api;
using DominoPontaDeQuina.Application;
using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Domain.Repositories;
using DominoPontaDeQuina.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection.");
var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Configure Jwt:Issuer.");
var audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Configure Jwt:Audience.");
var key = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configure Jwt:Key por variável de ambiente ou user-secrets.");
if (Encoding.UTF8.GetByteCount(key) < 32)
    throw new InvalidOperationException("Jwt:Key deve ter pelo menos 32 bytes UTF-8.");

builder.Services.AddDominoApplication();
builder.Services.AddDominoInfrastructure(options => options.UseSqlServer(connectionString));
builder.Services.AddSingleton<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
builder.Services.AddSingleton(new JwtTokenService(issuer, audience, key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            RequireSignedTokens = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var auth = app.MapGroup("/api/auth");

auth.MapPost("/registrar", async (Credenciais request, IUsuarioRepository usuarios,
    IPasswordHasher<Usuario> hasher, CancellationToken cancellationToken) =>
{
    var email = request.Email?.Trim().ToLowerInvariant();
    if (string.IsNullOrEmpty(email) || !new EmailAddressAttribute().IsValid(email) || email.Length > 320)
        return Results.BadRequest(new { erro = "Informe um e-mail válido." });
    if (string.IsNullOrEmpty(request.Senha) || request.Senha.Length < 8)
        return Results.BadRequest(new { erro = "A senha deve ter pelo menos 8 caracteres." });

    if (await usuarios.ObterPorEmailAsync(email, cancellationToken) is not null)
        return Results.Conflict(new { erro = "E-mail já cadastrado." });

    var usuario = new Usuario { Email = email };
    usuario.SenhaHash = hasher.HashPassword(usuario, request.Senha);
    await usuarios.AdicionarAsync(usuario, cancellationToken);
    return Results.Created($"/api/auth/me", new { usuario.Id, usuario.Email });
});

auth.MapPost("/login", async (Credenciais request, IUsuarioRepository usuarios,
    IPasswordHasher<Usuario> hasher, JwtTokenService tokens, CancellationToken cancellationToken) =>
{
    var email = request.Email?.Trim().ToLowerInvariant();
    var usuario = string.IsNullOrEmpty(email)
        ? null : await usuarios.ObterPorEmailAsync(email, cancellationToken);

    if (usuario is null || string.IsNullOrEmpty(request.Senha) ||
        hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, request.Senha) == PasswordVerificationResult.Failed)
        return Results.Unauthorized();

    return Results.Ok(tokens.Gerar(usuario));
});

auth.MapGet("/me", (ClaimsPrincipal principal) => Results.Ok(new
{
    Id = principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value,
    Email = principal.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value
})).RequireAuthorization();

app.Run();

/// <summary>Dados informados no cadastro ou login.</summary>
public sealed record Credenciais(string Email, string Senha);
