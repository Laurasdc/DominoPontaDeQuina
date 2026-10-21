# Laboratório xUnit, FluentAssertions e Moq

Execute a partir da raiz do repositório:

```powershell
dotnet test DominoPontaDeQuina.UnitTests
```

São 14 casos: 10 para pontuação e 4 para decisões do PartidaService.

## Pontuação

A classe `Core/Regras/PontuacaoDomino.cs` recebe a soma das pontas e retorna
`soma / 5` quando ela é múltipla de cinco. Demais somas retornam zero.
Os testes incluem explicitamente 10 → 2 e 15 → 3, zero, não múltiplos e
somas negativas. Soma negativa é rejeitada com ArgumentOutOfRangeException.
Esta classe isola a regra solicitada; a implementação completa da rodada,
que contém outros TODOs no repositório, não faz parte deste laboratório.

## Serviço e repositórios

Os testes usam o PartidaService real e mocks estritos das interfaces dos
repositórios, sem banco de dados nem Entity Framework. O método escolhido
é RegistrarLanceAsync:

- Partida em andamento e jogador existente: consulta partida, consulta jogador
  e salva exatamente um lance, nessa ordem. Confere retorno, identificador,
  data e propagação do CancellationToken.
- Partida inexistente: lança KeyNotFoundException, sem consultar jogador nem salvar.
- Partida finalizada: lança InvalidOperationException, sem consultar jogador nem salvar.
- Jogador inexistente: consulta as duas entidades, lança KeyNotFoundException
  e não salva.

Os cenários seguem Arrange, Act e Assert. FluentAssertions verifica valores e
exceções; Moq prepara os retornos e verifica as interações, incluindo ausência
de chamadas inesperadas. Em testes de exceção, Act & Assert usam Throw/ThrowAsync.

O projeto novo está separado dos testes existentes para permitir executar o
laboratório diretamente. Nenhum teste antigo é removido ou desativado.
