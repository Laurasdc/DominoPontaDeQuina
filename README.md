     # Domino Ponta de Quina

Projeto de implementação das regras do jogo Dominó Ponta de Quina, organizado em camadas.

## Estrutura da solução

- `DominoPontaDeQuina.Core`: regras de negócio, modelos, interfaces e enums do jogo.
- `DominoPontaDeQuina.Domain`: entidades persistentes e relacionamentos do domínio.
- `DominoPontaDeQuina.Infrastructure`: persistência com Entity Framework Core e mapeamentos Fluent API.
- `DominoPontaDeQuina.Migrations`: projeto independente para migrations do banco de dados.
- `DominoPontaDeQuina.Tests`: testes automatizados do domínio.
- `DominoPontaDeQuina.Api`: cadastro, login JWT e rota protegida para consultar a conta autenticada.

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

## Autenticação JWT

A API utiliza a tabela `Usuarios` já presente na migration inicial. Antes de executá-la, crie o banco com a migration do projeto `DominoPontaDeQuina.Migrations`. O cadastro armazena apenas o hash da senha produzido pelo `PasswordHasher<Usuario>`.

Configure uma chave secreta com pelo menos 32 bytes. No PowerShell, a partir da raiz do repositório:

```powershell
$env:Jwt__Key = ([guid]::NewGuid().ToString("N") + [guid]::NewGuid().ToString("N"))
dotnet run --project DominoPontaDeQuina.Api --urls http://localhost:5080
```

O exemplo gera uma chave para o processo atual; uma nova chave invalida os tokens emitidos anteriormente. Em ambientes permanentes, configure `Jwt__Key` em um gerenciador de segredos e use HTTPS. A conexão padrão é SQL Server LocalDB e pode ser sobrescrita por `ConnectionStrings__DefaultConnection`.

Rotas:

- `POST /api/auth/registrar`: JSON `{"email":"laura@example.com","senha":"SenhaForte123"}`; retorna 201 quando cadastrado e 409 quando o e-mail já existe.
- `POST /api/auth/login`: mesmo JSON; retorna `accessToken` e `expiresAt` se as credenciais forem válidas, ou 401.
- `GET /api/auth/me`: requer `Authorization: Bearer <accessToken>`; retorna os dados do token ou 401.

Para testar pelo PowerShell, abra outro terminal enquanto a API estiver rodando:

```powershell
$body = @{ email = "laura@example.com"; senha = "SenhaForte123" } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri http://localhost:5080/api/auth/registrar -ContentType application/json -Body $body
$login = Invoke-RestMethod -Method Post -Uri http://localhost:5080/api/auth/login -ContentType application/json -Body $body
Invoke-RestMethod -Uri http://localhost:5080/api/auth/me -Headers @{ Authorization = "Bearer $($login.accessToken)" }
```
