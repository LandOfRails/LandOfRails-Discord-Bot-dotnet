using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using LandOfRails_Discord_Bot_DOTNET.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Discord_Bot_DOTNET
{
    class Program
    {
        //context PW section = {File.ReadAllLines("Sensitive-data")[1]}
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            var token = await File.ReadAllLinesAsync("Sensitive-data");
            await using var services = ConfigureServices();
            var client = services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await client.LoginAsync(TokenType.Bot, token[0]);
            await client.StartAsync();

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            services.GetRequiredService<MessageHandlingService>().register();
            services.GetRequiredService<ReactionHandlingService>().register();
            services.GetRequiredService<PollHandlingService>().register();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());

            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<MessageHandlingService>()
                .AddSingleton<ReactionHandlingService>()
                .AddSingleton<PollHandlingService>()
                .AddSingleton<HttpClient>()
                .AddSingleton<lordiscordbotContext>()
                .BuildServiceProvider();
        }
    }
}
