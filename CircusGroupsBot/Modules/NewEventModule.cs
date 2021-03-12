using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CircusGroupsBot.Events;

namespace CircusGroupsBot.Modules
{
    public class NewEventModule : ModuleBase<SocketCommandContext>
    {
        [Command("newevent")]
        [Summary("Create a new event!")]
        public Task RunModuleAsync(string eventName, string dateandtime, string description = "", int tanks = 0, int healers = 0, int dds = 0)
        {
            var newEvent = new Event(Context.User, eventName, dateandtime, description, tanks, healers, dds);

            return ReplyAsync(newEvent.GetAnncouncementString());
        }
    }
}
