using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
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
                services.GetRequiredService<DiscordSocketClient>().Log += logger.Log;
                services.GetRequiredService<CommandService>().Log += logger.Log;

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
                .AddDbContext<CircusDbContext>(options => options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test"))
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Logger>()
                .BuildServiceProvider();
        }
    }
}
