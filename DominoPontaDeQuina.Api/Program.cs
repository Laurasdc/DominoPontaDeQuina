using DominoPontaDeQuina.Api.Filters;
using DominoPontaDeQuina.Application;
using DominoPontaDeQuina.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:DefaultConnection.");
builder.Services.AddDominoApplication();
builder.Services.AddDominoInfrastructure(options => options.UseSqlServer(connection));
builder.Services.AddControllers(options => options.Filters.Add<RegraDeUsoExceptionFilter>());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.MapControllers();
app.Run();

public partial class Program { }
