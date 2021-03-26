using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private CommandService services;

        public HelpModule(CommandService services)
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
            var eb = new EmbedBuilder();
            eb.WithTitle("Help");
            eb.WithDescription("Following this is a list of commands and descriptions of what they do");

            var commands = new Dictionary<string, string>();
            foreach(var module in services.Modules)
            {
                commands.Add(module.Commands[0].Aliases[0], module.Commands[0].Summary);
            }

            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            foreach(var command in commands)
            {
                sb.Append($"`{command.Key}`: {command.Value}{Environment.NewLine}{Environment.NewLine}");
            }

            eb.AddField("Commands", sb.ToString());

            return user.SendMessageAsync(embed: eb.Build());
        }
    }
}