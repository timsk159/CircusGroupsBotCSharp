using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Command to get list of other commands from the bot")]
        public Task RunModuleAsync()
        {
            var user = Context.User;
            if (user == null)
            {
                return null;
            }

            return user.SendMessageAsync(@"¯\_(ツ)_/¯");
        }
    }
}