using Microsoft.AspNetCore.Mvc.Testing;

// Localiza a API a partir de bin/Debug/net8.0 sem depender de uma solucao .sln.
[assembly: WebApplicationFactoryContentRoot(
    "DominoPontaDeQuina.Api, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null",
    "../../../../DominoPontaDeQuina.Api",
    "DominoPontaDeQuina.Api.csproj",
    "0")]
