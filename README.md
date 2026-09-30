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


## Laboratorio DI

Os contratos dos quatro repositories estao em `Domain/Repositories`. A camada
`Application/Services` recebe os contratos por construtor e coordena validacoes de
cadastro, referencias entre entidades, participacoes duplicadas e inicio/finalizacao
de partidas. As consultas LINQ continuam nos repositories.

`DominoPontaDeQuina.Console/Program.cs` registra o DbContext, os repositories,
os services e o `FluxoPrincipal` como scoped. O console recebe services por construtor;
o container cria as classes de aplicacao. Instanciacoes de entidades, opcoes do
container e a factory de design time das migrations continuam permitidas.

Execute a demonstracao isolada (SQLite em memoria, sem alterar seu banco):

```powershell
dotnet run --project DominoPontaDeQuina.Console -- --demo
```

Resultado esperado: `DI OK: cadastro, consultas LINQ, atualizacao, finalizacao e remocao.`

Execute os testes dos fluxos e das regras de uso:

```powershell
dotnet test DominoPontaDeQuina.DI.Tests
```

Para listar os dados do banco existente, execute na raiz da solucao:

```powershell
dotnet run --project DominoPontaDeQuina.Console
```

A conexao padrao e `Data Source=domino.db` e pode ser alterada pela variavel
`DOMINO_CONNECTION`. Aplique as migrations dos laboratorios anteriores antes de
executar contra um banco persistente. Este laboratorio nao altera o modelo do banco
nem exige nova migration. `HashSenha` e recebido pronto pelos services: o fluxo
`--demo` usa apenas um valor ficticio em um banco descartavel; autenticacao sera
implementada no laboratorio JWT.
