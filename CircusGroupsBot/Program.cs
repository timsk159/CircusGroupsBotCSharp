using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
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
                var myToken = Environment.GetEnvironmentVariable(Config.BOT_TOKEN_ENV_VAR);

                await client.LoginAsync(TokenType.Bot, myToken);
                await client.StartAsync();


                await services.GetRequiredService<CommandHandler>().InitAsync();
                services.GetRequiredService<ReactionHandler>().Init();
                services.GetRequiredService<MessageModificationHandler>().Init();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            var dbPass = Environment.GetEnvironmentVariable(Config.DB_PASSWORD_ENV_VAR);
            var dbUser = Environment.GetEnvironmentVariable(Config.DB_USER_ENV_VAR);

            if (string.IsNullOrEmpty(dbUser))
                dbUser = "root";

            var discordSocketConfig = new DiscordSocketConfig();
            discordSocketConfig.AlwaysDownloadUsers = true;

            var discordClient = new DiscordSocketClient(discordSocketConfig);

            return new ServiceCollection()
                .AddDbContextPool<CircusDbContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(
                        $"server=localhost;user={dbUser};password={dbPass};database=circusdb;charset=utf8mb4",
                        mySqlOptions => mySqlOptions
                            .ServerVersion(new Version(10, 3, 27), ServerType.MariaDb)
                            .CharSetBehavior(CharSetBehavior.NeverAppend))
                    .UseLoggerFactory(
                        LoggerFactory.Create(logging => logging
                                .AddConsole()
                                .AddFilter(level => level >= LogLevel.Information)))
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors())
                .AddSingleton<DiscordSocketClient>(discordClient)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Logger>()
                .AddSingleton<ReactionHandler>()
                .AddSingleton<MessageModificationHandler>()
                .BuildServiceProvider();
        }
    }
}
