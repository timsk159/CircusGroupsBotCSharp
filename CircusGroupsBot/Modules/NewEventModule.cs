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
        public Task RunModuleAsync(string eventName, string dateandtime, string description = "", int tanks = 0, int healers = 0, int dds = 0, int runners = 0)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Creating new event {eventName}, {dateandtime}, {description}, {tanks}, {healers}, {dds}, {runners}"));
            var newEvent = new Event(Context.User, eventName, dateandtime, 0UL, description, tanks, healers, dds, runners);

            var allRoleReactionsEmoji = Enum.GetValues(typeof(Role)).OfType<Role>().Select(e => e.GetEmoji()).ToList();

            if(newEvent.Signups.SingleOrDefault(e => e.Role == Role.Runner && e.IsRequired) == null)
            {
                allRoleReactionsEmoji.RemoveAll(e => e.Name == Role.Runner.GetEmoji().Name);
            }

            var messageTask = ReplyAsync(newEvent.GetAnnouncementString());
            messageTask.ContinueWith(cont => newEvent.UpdateSignupsOnMessageAsync(Context.Channel));
            messageTask.ContinueWith(cont => cont.Result.AddReactionsAsync(allRoleReactionsEmoji.ToArray()));

            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Assigning event with ID {newEvent.EventId} a messageID of {messageTask.Result.Id}"));
            newEvent.EventMessageId = messageTask.Result.Id;

            DbContext.Events.Add(newEvent);
            DbContext.SaveChanges();

            return messageTask;
        }
    }
}
