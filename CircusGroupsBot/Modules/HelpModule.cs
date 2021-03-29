using CircusGroupsBot.Events;
using CircusGroupsBot.Models;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Summary("Get help with using the bot")]
        public Task RunModuleAsync()
        {
            var user = Context.User;
            if (user == null)
            {
                return null;
            }
            var eb = new EmbedBuilder();
            eb.WithTitle("Help");
            eb.WithDescription($"Following this is a list of commands and descriptions of what they do{Environment.NewLine}");

            var commands = new List<Command>();
            foreach (var module in services.Modules)
            {
                module.Commands.Select(x => x.Aliases[0]).Distinct().ToList().ForEach(command =>
                {
                    var summaries = module.Commands.Where(cmd => cmd.Aliases[0].Equals(command)).Select(cmd => cmd.Summary).ToList();
                    commands.Add(new Command()
                    {
                        Alias = command,
                        Commands = summaries
                    });
                });
            }

            foreach (var command in commands)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < command.Commands.Count; i++)
                {
                    sb.Append($"- {command.Commands[i]}{Environment.NewLine}");
                }
                sb.Append(Environment.NewLine);
                eb.AddField($"${command.Alias}", sb.ToString());
            }

            eb.AddField("Signing up", HowToSignUp());

            return user.SendMessageAsync(embed: eb.Build());
        }

        private string HowToSignUp()
        {
            var sb = new StringBuilder();
            sb.Append("To sign up for a new event react with the appropriate emoji attached to the event message");
            var allRoles = Enum.GetValues(typeof(Role)).OfType<Role>();
            foreach(var role in allRoles)
            {
                sb.Append(Environment.NewLine);
                sb.Append($"{role.GetEmoji()} is for {role.GetName()}");
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}