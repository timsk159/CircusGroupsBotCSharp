using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CircusGroupsBot
{
    class Program
    {
        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        private Program() { }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var logger = services.GetRequiredService<Logger>();
                services.GetRequiredService<DiscordSocketClient>().Log += logger.LogAsync;
                services.GetRequiredService<CommandService>().Log += logger.LogAsync;

                var client = services.GetRequiredService<DiscordSocketClient>();
                var myToken = Environment.GetEnvironmentVariable("circusBotToken");

                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("circusBotToken"));
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Logger>()
                .BuildServiceProvider();
        }
    }
}
