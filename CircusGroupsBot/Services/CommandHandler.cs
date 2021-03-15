using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CircusGroupsBot.Services
{
    public class CommandHandler
    {
        private int argPos = 0;
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;
        private readonly IServiceProvider services;

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
        {
            this.commands = commands;
            this.services = services;
            this.client = client;

            commands.CommandExecuted += CommandExecutedAsync;
            client.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitAsync()
        {
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: services);
        }

        private async Task MessageReceivedAsync(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;

            if (message == null ||
                !(message.HasCharPrefix('$', ref argPos) ||
                message.HasMentionPrefix(client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
            {
                return;
            }

            var context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(context, argPos, services);
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (!command.IsSpecified || result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"error: {result}");
        }
    }
}