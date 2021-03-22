using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CircusGroupsBot.Events;
using CircusGroupsBot.Services;
using Discord;
using System.Linq;

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
            var newEvent = new Event(Context.User, eventName, dateandtime, 0UL, description, tanks, healers, dds);

            var allRoleReactions = Enum.GetValues(typeof(Role)).OfType<Role>().Select(e => e.GetEmoji());

            var messageTask = ReplyAsync(newEvent.GetAnnouncementString());
            messageTask.ContinueWith(continuation => newEvent.UpdateSignupsOnMessageAsync(Context.Channel));
            messageTask.ContinueWith(continuationAction => continuationAction.Result.AddReactionsAsync(allRoleReactions.ToArray()));


            newEvent.EventMessageId = messageTask.Result.Id;

            DbContext.Events.Add(newEvent);
            DbContext.SaveChanges();

            

            return messageTask;
        }
    }
}
