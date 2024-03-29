﻿using CircusGroupsBot.Events;
using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
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

        //NOTE:
        //Due to ambigious parsing between int and string, the priority on the overloads here is very important, no touchy!

        [Command("newevent")]
        [Summary("\"Event Name\" \"Date\"")]
        [Priority(2)]
        public Task RunNewEvent(string eventName, string dateandtime)
        {
            return RunNewEvent(eventName, dateandtime, "", 0, 0, 0, 0);
        }

        [Command("newevent")]
        [Summary("\"Event Name\" \"Date\" \"Description\"")]
        [Priority(1)]
        public Task RunNewEvent(string eventName, string dateandtime, string description)
        {
            return RunNewEvent(eventName, dateandtime, description, 0, 0, 0, 0);
        }

        [Command("newevent")]
        [Summary("\"Event Name\" \"Date\" 1 1 1")]
        [Priority(3)]
        public Task RunNewEvent(string eventName, string dateandtime, int tanks, int healers, int dds)
        {
            return RunNewEvent(eventName, dateandtime, "", tanks, healers, dds, 0);
        }

        [Command("newevent")]
        [Summary("\"Event Name\" \"Date\" 1 1 1 1")]
        [Priority(5)]
        public Task RunNewEvent(string eventName, string dateandtime, int tanks, int healers, int dds, int runners)
        {
            return RunNewEvent(eventName, dateandtime, "", tanks, healers, dds, runners);
        }

        [Command("newevent")]
        [Summary("\"Event Name\" \"Date\" \"Description\" 1 1 1 1")]
        [Priority(4)]
        public Task RunNewEvent(string eventName, string dateandtime, string description, int tanks, int healers, int dds)
        {
            return RunNewEvent(eventName, dateandtime, description, tanks, healers, dds, 0);
        }

        [Command("newevent")]
        [Summary("\"Event Name\" \"Date\" \"Description\" 1 1 1 1")]
        [Priority(6)]
        public Task RunNewEvent(string eventName, string dateandtime, string description, int tanks, int healers, int dds, int runners)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Creating new event {eventName}, {dateandtime}, {description}, {tanks}, {healers}, {dds}, {runners}"));

            if(tanks + healers + dds + runners > 16)
            {
                return ReplyAsync("You asked for too many people in your event. The maximum group size is 16");
            }

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
            
            messageTask.ContinueWith(cont => cont.Result.PinAsync());
            messageTask.ContinueWith(cont => newEvent.UpdateSignupsOnMessageAsync(cont.Result, Context.Guild));
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
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewEvent", $"Modifying event {existingEvent.EventName}"));

            existingEvent.EventName = newEvent.EventName;
            existingEvent.DateAndTime = newEvent.DateAndTime;
            existingEvent.Description = newEvent.Description;

            var newReserves = new List<Signup>();
            existingEvent.TransferSignups(newEvent, out newReserves);

            //Anyone that was moved to reserved needs to be notified, and their old reaction removed
            foreach(var newReserve in newReserves)
            {
                var getUserTask = Context.Channel.GetUserAsync(newReserve.UserId);
                getUserTask.ContinueWith(t => t.Result.SendMessageAsync($"You were moved to reserve as {existingEvent.EventName} was modified, and no longer had room for your role"));
                var emotesToRemoveForUser = new IEmote[] { Role.DD.GetEmoji(), Role.Healer.GetEmoji(), Role.Tank.GetEmoji() };
                getUserTask.ContinueWith(t => eventMessage.RemoveReactionsAsync(t.Result, emotesToRemoveForUser));
            }

            //Remove reactions that are no longer needed
            //There might be an easier way to write this code? It seems really verbose... Maybe ternaries? I dunno
            if (newEvent.Signups.Any(e => e.IsRequired))
            {
                var hasTanks = newEvent.Signups.Any(e => e.Role == Role.Tank && e.IsRequired);
                var hasHealers = newEvent.Signups.Any(e => e.Role == Role.Healer && e.IsRequired);
                var hasDDs = newEvent.Signups.Any(e => e.Role == Role.DD && e.IsRequired);

                var emotesToRemoveForBot = new List<IEmote>();
                if (!hasTanks)
                {
                    emotesToRemoveForBot.Add(Role.Tank.GetEmoji());
                }
                if (!hasHealers)
                {
                    emotesToRemoveForBot.Add(Role.Healer.GetEmoji());
                }
                if (!hasDDs)
                {
                    emotesToRemoveForBot.Add(Role.DD.GetEmoji());
                }
                eventMessage.RemoveReactionsAsync(Context.Client.CurrentUser, emotesToRemoveForBot.ToArray());
            }

            DbContext.SaveChanges();

            var messageTask = eventMessage.ModifyAsync(e => e.Embed = GetEmbedForEvent(existingEvent, leaderUser));
            messageTask.ContinueWith(cont => existingEvent.UpdateSignupsOnMessageAsync(eventMessage, Context.Guild));
            messageTask.ContinueWith(cont => existingEvent.AddReactionsToMessageAsync(eventMessage));
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