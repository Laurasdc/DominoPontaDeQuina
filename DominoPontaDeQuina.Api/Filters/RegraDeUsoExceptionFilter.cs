using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DominoPontaDeQuina.Api.Filters;

public sealed class RegraDeUsoExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var status = context.Exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            ArgumentException => StatusCodes.Status400BadRequest,
            InvalidOperationException => StatusCodes.Status409Conflict,
            _ => (int?)null
        };
        if (status is null)
            return;
        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = status switch
            {
                404 => "Recurso nao encontrado",
                400 => "Dados invalidos",
                _ => "Operacao nao permitida"
            },
            Detail = context.Exception.Message
        }) { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
