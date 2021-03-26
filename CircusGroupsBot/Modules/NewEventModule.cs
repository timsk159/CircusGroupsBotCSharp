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
        [Priority(2)]
        public Task RunNewEvent(string eventName, string dateandtime, string description)
        {
            return RunNewEvent(eventName, dateandtime, description, 0, 0, 0, 0);
        }

        [Command("newevent")]
        [Summary("Create a new event!")]
        [Priority(1)]
        public Task RunNewEvent(string eventName, string dateandtime)
        {
            return RunNewEvent(eventName, dateandtime, "", 0, 0, 0, 0);
        }

        [Command("newevent")]
        [Summary("Create a new event!")]
        [Priority(3)]
        public Task RunNewEvent(string eventName, string dateandtime, int tanks, int healers, int dds, int runners = 0)
        {
            return RunNewEvent(eventName, dateandtime, "", tanks, healers, dds, runners);
        }

        [Command("newevent")]
        [Summary("Create a new event!")]
        [Priority(4)]
        public Task RunNewEvent(string eventName, string dateandtime, string description, int tanks, int healers, int dds, int runners = 0)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Creating new event {eventName}, {dateandtime}, {description}, {tanks}, {healers}, {dds}, {runners}"));
            var newEvent = new Event(Context.User, eventName, dateandtime, 0UL, description, tanks, healers, dds, runners);

            return CreateEvent(newEvent, Context.User);
        }



        [Command("neweventbytemplate")]
        [Summary("Create a new event based on a pre-created template")]
        public Task RunNewEventByTemplate(string templateName, string eventName, string dateandtime, string description = "")
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEventByTemplate", $"Creating new event from template {templateName} {eventName}, {dateandtime}, {description}"));

            var template = DbContext.Templates.FirstOrDefault(e => e.TemplateName == templateName);
            if(template == null)
            {
                return ReplyAsync($"Template with name {templateName} was not found");
            }

            var newEvent = new Event(Context.User, eventName, dateandtime, 0UL, description, template.Tanks, template.Healers, template.DDs, template.Runners);

            return CreateEvent(newEvent, Context.User);
        }

        private Task CreateEvent(Event newEvent, IUser leaderUser)
        {
            var eb = new EmbedBuilder();
            eb.WithTitle(newEvent.EventName);
            eb.WithDescription(newEvent.GetAnnouncementString());
            eb.WithAuthor(leaderUser);
            eb.WithCurrentTimestamp();

            var messageTask = ReplyAsync(message: "@everyone", embed: eb.Build());

            messageTask.ContinueWith(cont => newEvent.UpdateSignupsOnMessageAsync(cont.Result));
            messageTask.ContinueWith(cont => newEvent.AddReactionsToMessageAsync(cont.Result));

            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Assigning event with ID {newEvent.EventId} a messageID of {messageTask.Result.Id}"));
            newEvent.EventMessageId = messageTask.Result.Id;

            DbContext.Events.Add(newEvent);
            DbContext.SaveChanges();

            return messageTask;
        }
    }
}
