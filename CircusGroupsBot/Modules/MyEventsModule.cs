using Discord.Commands;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class MyEventsModule : ModuleBase<SocketCommandContext>
    {
        [Command("myevents")]
        [Summary("Command to get upcoming events for the person querying the bot")]
        public Task RunModuleAsync()
        {
            return ReplyAsync(@"¯\_(ツ)_/¯");
        }
    }
}