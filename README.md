     # Domino Ponta de Quina

Projeto de implementação das regras do jogo Dominó Ponta de Quina, organizado em camadas.

## Estrutura da solução

- `DominoPontaDeQuina.Core`: regras de negócio, modelos, interfaces e enums do jogo.
- `DominoPontaDeQuina.Domain`: entidades persistentes e relacionamentos do domínio.
- `DominoPontaDeQuina.Infrastructure`: persistência com Entity Framework Core e mapeamentos Fluent API.
- `DominoPontaDeQuina.Migrations`: projeto independente para migrations do banco de dados.
- `DominoPontaDeQuina.Tests`: testes automatizados do domínio.

## Persistência

O banco utilizado é SQL Server LocalDB. O `DominoDbContext` está em `DominoPontaDeQuina.Infrastructure` e possui as entidades:

- `Usuario`, utilizada para autenticação;
- `Partida`;
- `Jogador`;
- `ParticipacaoPartida`, associação entre jogador e partida com seus pontos;
- `Lance`, registro de lances com timestamp;
- `Ranking`, contador de vitórias por jogador.

As chaves estrangeiras ficam implícitas nas entidades e são configuradas como shadow properties pelo Fluent API.

## Dependências entre camadas

`Domain` não depende de nenhuma camada e contém as entidades e os contratos dos repositórios. `Application` depende apenas de `Domain` e implementa os casos de uso. `Infrastructure` depende de `Application` e `Domain` para implementar os repositórios com EF Core. A composição é feita por injeção de dependência.

### Configuração da conexão

A connection string padrão fica em `DominoPontaDeQuina.Migrations/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=DominoPontaDeQuina;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

Para sobrescrevê-la sem alterar o arquivo, utilize a variável `ConnectionStrings__DefaultConnection`.

### Migrations

A partir da raiz do repositório:

```bash
dotnet ef database update \
  --project DominoPontaDeQuina.Migrations \
  --startup-project DominoPontaDeQuina.Migrations \
  --context DominoDbContext
```

Para criar uma nova migration:

```bash
dotnet ef migrations add NomeDaMigration \
  --project DominoPontaDeQuina.Migrations \
  --startup-project DominoPontaDeQuina.Migrations \
  --context DominoDbContext \
  --output-dir Persistence/Migrations
```

## Build e testes

```bash
dotnet restore DominoPontaDeQuina.slnx
dotnet build DominoPontaDeQuina.slnx
dotnet test DominoPontaDeQuina.Tests/DominoPontaDeQuina.Tests.csproj
```

O workflow do GitHub Actions executa restore, build e testes. SonarCloud, geração de relatórios e artefatos não fazem parte da pipeline atual. Falhas dos testes não bloqueiam o workflow.


## Laboratorio API

O projeto `DominoPontaDeQuina.Api` referencia Application e Infrastructure e expoe
as seis operacoes de `IPartidaService`. O controller recebe o service por construtor;
as consultas LINQ permanecem nos repositories. O Program registra as dependencias
pelas extensoes `AddDominoApplication` e `AddDominoInfrastructure`, alem dos
controllers e Swagger. DTOs evitam expor senhas e ciclos entre entidades.

| Metodo | Rota | Operacao |
| --- | --- | --- |
| POST | /api/partidas | Iniciar partida (pontuacaoAlvo, padrao 50) |
| POST | /api/partidas/{id}/jogadores | Registrar jogador (nome, usuarioId opcional) |
| POST | /api/partidas/{id}/lances | Registrar lance (jogadorId) |
| GET | /api/partidas/{id}/status | Consultar status |
| GET | /api/partidas/historico | Consultar historico |
| GET | /api/ranking | Consultar ranking |

Respostas: 201 para criacao, 200 para consulta, 400 para dados invalidos, 404
para recursos nao encontrados e 409 para uma operacao recusada pelo service.
As regras de jogo sao as implementadas no service da branch `dev/efcore-di`;
a API nao implementa novas regras de pontuacao ou finalizacao.

Na raiz da solucao, aplique a migration existente ao SQL Server LocalDB:

```powershell
dotnet ef database update --project DominoPontaDeQuina.Migrations --startup-project DominoPontaDeQuina.Migrations --context DominoDbContext
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project DominoPontaDeQuina.Api --urls http://localhost:5080
```

Abra `http://localhost:5080/swagger` para testar. A conexao pode ser alterada
em `Api/appsettings.json` ou por `ConnectionStrings__DefaultConnection`.
O projeto de migrations foi configurado como executavel e a factory antiga que
referenciava Repository foi removida. A factory ativa le a configuracao copiada
para a pasta de saida do projeto. Nao ha alteracao de esquema nem nova migration.

Exemplo em outro terminal PowerShell:

```powershell
$base = "http://localhost:5080"
$partida = Invoke-RestMethod -Method Post -Uri "$base/api/partidas" -ContentType "application/json" -Body '{"pontuacaoAlvo":50}'
$jogador = Invoke-RestMethod -Method Post -Uri "$base/api/partidas/$($partida.id)/jogadores" -ContentType "application/json" -Body '{"nome":"Laura"}'
$body = @{ jogadorId = $jogador.id } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "$base/api/partidas/$($partida.id)/lances" -ContentType "application/json" -Body $body
Invoke-RestMethod -Uri "$base/api/partidas/$($partida.id)/status"
Invoke-RestMethod -Uri "$base/api/partidas/historico"
Invoke-RestMethod -Uri "$base/api/ranking"
```

Testes HTTP com os services e repositories reais e SQLite em memoria, sem
alterar o banco LocalDB:

```powershell
dotnet test DominoPontaDeQuina.Api.Tests
```

Os cinco testes verificam criacao e persistencia de partida/jogador/lance,
status e historico, dados invalidos (400), partida inexistente (404) e ranking.
Este laboratorio expoe as operacoes sem autenticacao; JWT pertence a entrega seguinte.
