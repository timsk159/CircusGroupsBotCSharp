using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CircusGroupsBot.Events;
using CircusGroupsBot.Services;
using Discord;

namespace CircusGroupsBot.Modules
{
    public class NewEventModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;
        public NewEventModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }


        [Command("newevent")]
        [Summary("Create a new event!")]
        public Task RunModuleAsync(string eventName, string dateandtime, string description = "", int tanks = 0, int healers = 0, int dds = 0)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Creating new event {eventName}, {dateandtime}, {description}, {tanks}, {healers}, {dds}"));
            var newEvent = new Event(Context.User, eventName, dateandtime, description, tanks, healers, dds);

            DbContext.Events.Add(newEvent);
            DbContext.SaveChanges();

            return ReplyAsync(newEvent.GetAnnouncementString());
        }
    }
}
