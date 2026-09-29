# Domino Ponta de Quina

## Projetos

- `DominoPontaDeQuina.Core`: regras e fluxo do jogo.
- `DominoPontaDeQuina.Domain`: entidades e enums persistentes.
- `DominoPontaDeQuina.Repository`: `DominoDbContext` e repositorios EF Core.
- `DominoPontaDeQuina.Migrations`: aplicacao console usada como startup project para migrations.
- `DominoPontaDeQuina.Tests`: testes automatizados do nucleo do jogo.

## Modelo persistente

`Usuario` representa a conta do aplicativo cliente e pode possuir varios `Jogador`, que sao perfis de jogo.
`Partida` representa uma partida armazenada para consulta de historico. `ParticipacaoPartida` liga um jogador a uma partida e registra sua posicao, pontuacao e resultado.
`Usuario` e `Jogador` usam Data Annotations. Os tres relacionamentos entre as entidades sao configurados no `DominoDbContext` com Fluent API.

Os repositorios `UsuarioRepository`, `JogadorRepository`, `PartidaRepository` e `ParticipacaoPartidaRepository` herdam as operacoes CRUD de `RepositoryBase`. As consultas especificas utilizam LINQ e retornam dados sem rastreamento.

Esta etapa prepara a persistencia e o futuro fluxo de autenticacao. API, endpoints, autenticacao e JWT estao fora do escopo.

## Pre-requisitos

- .NET 8 SDK
- Ferramenta `dotnet-ef` 8.x (`dotnet tool install --global dotnet-ef --version 8.*`)

## Restaurar e compilar

```bash
dotnet restore
dotnet build
```

## Migrations

O `DominoDbContextFactory` configura SQLite com `Data Source=domino.db`. Execute os comandos na raiz do repositorio para criar o banco nessa pasta. Use `DominoPontaDeQuina.Migrations` como startup project e `DominoPontaDeQuina.Repository` como projeto do contexto:

```bash
dotnet ef migrations add Inicial \
  --project DominoPontaDeQuina.Repository \
  --startup-project DominoPontaDeQuina.Migrations

dotnet ef database update \
  --project DominoPontaDeQuina.Repository \
  --startup-project DominoPontaDeQuina.Migrations
```

O banco SQLite local `domino.db` e ignorado pelo Git.

Depois de aplicar a migration inicial do laboratorio anterior, gere a migration deste laboratorio:

```bash
dotnet ef migrations add RenomeiaPartidasERelacionamentos \
  --project DominoPontaDeQuina.Repository \
  --startup-project DominoPontaDeQuina.Migrations

dotnet ef database update \
  --project DominoPontaDeQuina.Repository \
  --startup-project DominoPontaDeQuina.Migrations
```

Antes de aplicar uma migration de renomeacao a um banco com dados, confira se o codigo gerado preserva os registros existentes.
