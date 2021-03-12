using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Commands
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("testcommand")]
        [Summary("Quick test to make sure the bot is alive!")]
        public Task RunModuleAsync()
        {
            return ReplyAsync("Hello!");
        }
    }
}
