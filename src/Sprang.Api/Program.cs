using Sprang.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCore();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/movimentacoes", async (IMovimentacaoHandler handler, MovimentacaoCommand command, CancellationToken cancellationToken) =>
{
    await handler.Handle(command, cancellationToken);
    return Results.Created();
})
.WithName("PostMovimentacoes")
.WithOpenApi();

app.Run();

public partial class Program
{
}