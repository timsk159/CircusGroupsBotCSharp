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

        private readonly DiscordSocketClient discord;
        private readonly CommandService commandService;
        private readonly Logger logger;

        private Program()
        {
            discord = new DiscordSocketClient();
            commandService = new CommandService();
            logger = new Logger(discord, commandService);
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                var myToken = Environment.GetEnvironmentVariable("circusBotToken");

                await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("circusBotToken"));
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        //Bad, black-magic, DI here.
        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>(discord)
                .AddSingleton<CommandService>(commandService)
                .AddSingleton<CommandHandler>()
                .AddSingleton<ILogger>(logger)
                .BuildServiceProvider();
        }
    }
}
