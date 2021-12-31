using System.Net.Sockets;
using MediatR;
using Microsoft.Extensions.Configuration;
using TwitchBox.Commands.Abstractions;

namespace TwitchBox.Commands
{
    public class StartBotCommandHandler : IRequestHandler<StartBotCommand>
    {
        private readonly IConfiguration _configuration;

        public StartBotCommandHandler(IConfiguration configuration)
            => _configuration = configuration;

        public async Task<Unit> Handle(StartBotCommand request, CancellationToken cancellationToken)
        {
            var client = new TcpClient();
            await client.ConnectAsync(_configuration["BotConfig:Url"], int.Parse(_configuration["BotConfig:Port"]));

            var reader = new StreamReader(client.GetStream());
            var writer = new StreamWriter(client.GetStream()) { NewLine = "\r\n", AutoFlush = true };

            await writer.WriteLineAsync($"PASS {_configuration["TwitchPassword"]}");
            await writer.WriteLineAsync($"NICK {_configuration["BotConfig:Username"]}");
            await writer.WriteLineAsync($"JOIN #{_configuration["BotConfig:JoinAt"]}");
            await writer.WriteLineAsync($"PRIVMSG #{_configuration["BotConfig:JoinAt"]} :ChatBot iniciado com sucesso!");

            if (client.Connected)
            {
                if (reader is null || writer is null)
                    throw new Exception("Reader or Writer is null");

                while (true)
                {
                    string line = await reader.ReadLineAsync();
                    string[] splittedLine = line.Split(" :");
                    string code = splittedLine[0];
                    string message = splittedLine.Length > 1 ? splittedLine[1] : splittedLine[0];

                    if (line.StartsWith("PING"))
                    {
                        await writer.WriteLineAsync($"PONG {message}");
                        continue;
                    }
                    else if (!line.Contains("PRIVMSG"))
                        continue;

                    string user = (code.Split("!"))[0].Trim().Replace(":", "");

                    Console.WriteLine($"{user} said: {message}");

                    // if (message is not "-" && message is not ">")
                    //     Console.WriteLine($"ChzD7:{(code == "PING" ? $" {code}" : "")} {message}");
                }
            }

            return await Task.FromResult(Unit.Value);
        }
    }
}