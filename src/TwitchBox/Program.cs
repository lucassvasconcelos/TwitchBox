using MediatR;
using TwitchBox.Commands;
using TwitchBox.Commands.Abstractions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCommands();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var mediator = app.Services.GetService<IMediator>();

app.MapPost("/bot/start", () => mediator.Send(new StartBotCommand()));

await app.RunAsync();