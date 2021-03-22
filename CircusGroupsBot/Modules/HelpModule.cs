using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private IServiceProvider services;

        public HelpModule(IServiceProvider services)
        {
            this.services = services;
        }

        [Command("help")]
        [Summary("Command to get list of other commands from the bot")]
        public Task RunModuleAsync()
        {
            var user = Context.User;
            if (user == null)
            {
                return null;
            }

            var commandService = services.GetRequiredService<CommandService>();
            var commands = new Dictionary<string, string>();
            foreach(var module in commandService.Modules)
            {
                commands.Add(module.Commands[0].Aliases[0], module.Commands[0].Summary);
            }

            var sb = new StringBuilder();
            sb.Append("Commands\r\n-------\r\n");
            foreach(var command in commands)
            {
                sb.Append($"{command.Key}: {command.Value}\r\n");
            }

            return user.SendMessageAsync(sb.ToString());
        }
    }
}