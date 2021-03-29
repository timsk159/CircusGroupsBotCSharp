using CircusGroupsBot.Events;
using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

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
        [Summary("\"name\" \"date\"")]
        [Priority(1)]
        public Task RunNewEvent(string eventName, string dateandtime)
        {
            return RunNewEvent(eventName, dateandtime, "", 0, 0, 0, 0);
        }

        [Command("newevent")]
        [Summary("\"name\" \"date\" \"description\"")]
        [Priority(2)]
        public Task RunNewEvent(string eventName, string dateandtime, string description)
        {
            return RunNewEvent(eventName, dateandtime, description, 0, 0, 0, 0);
        }

        [Command("newevent")]
        [Summary("\"name\" \"date\" 1 1 1 1")]
        [Priority(3)]
        public Task RunNewEvent(string eventName, string dateandtime, int tanks, int healers, int dds, int runners = 0)
        {
            return RunNewEvent(eventName, dateandtime, "", tanks, healers, dds, runners);
        }

        [Command("newevent")]
        [Summary("\"name\" \"date\" \"description\" 1 1 1 1")]
        [Priority(4)]
        public Task RunNewEvent(string eventName, string dateandtime, string description, int tanks, int healers, int dds, int runners = 0)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Creating new event {eventName}, {dateandtime}, {description}, {tanks}, {healers}, {dds}, {runners}"));

            var newEvent = new Event(Context.User, eventName, dateandtime, 0UL, description, tanks, healers, dds, runners);

            //Check for editing a message to modify an event
            var existingEvent = DbContext.Events.AsQueryable().Where(e => e.CommandMessageId == Context.Message.Id).FirstOrDefault();
            if (existingEvent != null)
            {
                var eventMessage = Context.Channel.GetMessageAsync(existingEvent.EventMessageId);
                eventMessage.ContinueWith(t => ModifyEvent(existingEvent, newEvent, (IUserMessage)t.Result, Context.User));
                return eventMessage;
            }
            else
            {
                return CreateEvent(newEvent, Context.User, Context.Message);
            }
        }

        [Command("neweventbytemplate")]
        [Summary("\"Template Name\" \"Event Name\" \"Date\" \"Description\"")]
        public Task RunNewEventByTemplate(string templateName, string eventName, string dateandtime, string description = "")
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEventByTemplate", $"Creating new event from template {templateName} {eventName}, {dateandtime}, {description}"));

            var template = DbContext.Templates.FirstOrDefault(e => e.TemplateName == templateName);
            if(template == null)
            {
                return ReplyAsync($"Template with name {templateName} was not found");
            }

            return RunNewEvent(eventName, dateandtime, description, template.Tanks, template.Healers, template.DDs, template.Runners);
        }

        private Task CreateEvent(Event newEvent, IUser leaderUser, IMessage commandMessage)
        {
            var embed = GetEmbedForEvent(newEvent, leaderUser);
            var messageTask = ReplyAsync(message: "@everyone", embed: embed);

            messageTask.ContinueWith(cont => newEvent.UpdateSignupsOnMessageAsync(cont.Result));
            messageTask.ContinueWith(cont => newEvent.AddReactionsToMessageAsync(cont.Result));

            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Assigning event with ID {newEvent.EventId} a messageID of {messageTask.Result.Id}"));
            newEvent.EventMessageId = messageTask.Result.Id;
            newEvent.CommandMessageId = commandMessage.Id;

            DbContext.Events.Add(newEvent);
            DbContext.SaveChanges();

            return messageTask;
        }

        private Task ModifyEvent(Event existingEvent, Event newEvent, IUserMessage eventMessage, IUser leaderUser)
        {
            existingEvent.EventName = newEvent.EventName;
            existingEvent.DateAndTime = newEvent.DateAndTime;
            existingEvent.Description = newEvent.Description;

            //TODO: Move existing signups
            existingEvent.Signups = newEvent.Signups;

            DbContext.SaveChanges();

            var messageTask = eventMessage.ModifyAsync(e => e.Embed = GetEmbedForEvent(existingEvent, leaderUser));
            messageTask.ContinueWith(cont => newEvent.UpdateSignupsOnMessageAsync(eventMessage));
            messageTask.ContinueWith(cont => newEvent.AddReactionsToMessageAsync(eventMessage));
            return messageTask;
        }

        private Embed GetEmbedForEvent(Event evnt, IUser leaderUser)
        {
            var eb = new EmbedBuilder();
            eb.WithTitle(evnt.EventName);
            eb.WithDescription(evnt.GetAnnouncementString());
            eb.WithAuthor(leaderUser);
            eb.WithCurrentTimestamp();
            return eb.Build();
        }
    }
}