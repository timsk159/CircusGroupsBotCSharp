﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Discord;
using Discord.WebSocket;

namespace CircusGroupsBot.Events
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public ulong LeaderUserID { get; set; }
        public string EventName { get; set; }
        public string DateAndTime { get; set; }
        public string Description { get; set; }
        public ulong EventMessageId { get; set; }
        public List<Signup> Signups { get; set; }


        private Event(ulong leaderUserID, string eventName, string dateAndTime, ulong eventMessageId, string description = "")
        {
            this.LeaderUserID = leaderUserID;
            this.EventName = eventName;
            this.DateAndTime = dateAndTime;
            this.Description = description;
            this.EventMessageId = eventMessageId;
        }

        public Event(IUser leader, string eventName, string dateAndTime, ulong eventMessageId, string description = "",
    int tanks = 0, int healers = 0, int dds = 0, int runners = 0) : this(leader.Id, eventName, dateAndTime, eventMessageId, description, tanks, healers, dds, runners) { }

        public Event(ulong leaderUserID, string eventName, string dateAndTime, ulong eventMessageId, string description = "",
    int tanks = 0, int healers = 0, int dds = 0, int runners = 0) : this(leaderUserID, eventName, dateAndTime, eventMessageId, description)
        {
            Signups = new List<Signup>();

            for(int i = 0; i < tanks; ++i)
            {
                Signups.Add(new Signup(Role.Tank, true));
            }
            for(int i = 0; i < healers; ++i)
            {
                Signups.Add(new Signup(Role.Healer, true));
            }
            for (int i = 0; i < dds; ++i)
            {
                Signups.Add(new Signup(Role.DD, true));
            }
            for (int i = 0; i < runners; ++i)
            {
                Signups.Add(new Signup(Role.Runner, true));
            }
        }

        public Event(ulong leaderUserID, string eventName, string dateAndTime, ulong eventMessageId, List<Signup> signups, string description = "") : this(leaderUserID, eventName, dateAndTime, eventMessageId, description)
        {
            this.Signups = signups;
        }

        public string GetAnnouncementString()
        {
            return $@"@everyone
{EventName}
Scheduled For: {DateAndTime}

Leader: <@{LeaderUserID}>

{Description}

";
        }

        public string GetReactionInstructionsString()
        {
            var returnStr = "\nReact With:\n";
            foreach(Role val in Enum.GetValues(typeof(Role)))
            {
                if (val == Role.Runner)
                {
                    if (Signups.Any(e => e.Role == Role.Runner && e.IsRequired))
                    {
                        returnStr += $"{val.GetEmoji().Name} to sign up as {val.GetName()}\n";
                    }
                }
                else
                {
                    returnStr += $"{val.GetEmoji().Name} to sign up as {val.GetName()}\n";
                }
            }
            return returnStr;
        }

        public bool TryAddSignup(Role role, ulong userID)
        {
            if(role != Role.Maybe && Signups.Any(e => e.IsRequired == true))
            {
                var freeSlot = Signups.FirstOrDefault(e => e.Role == role && !e.SignupFilled());
                if(freeSlot != null)
                {
                    freeSlot.UserId = userID;
                    return true;
                }
                return false;
            }
            else
            {
                Signups.Add(new Signup(role, false, userID));
                return true;
            }
        }

        public void RemoveSignup(Role role, ulong userID)
        {
            RemoveSignup(Signups.FirstOrDefault(e => e.Role == role && e.UserId == userID));
        }

        public void RemoveSignup(Signup signupToRemove)
        {
            if(signupToRemove == null)
            {
                return;
            }
            if(signupToRemove.IsRequired == true)
            {
                signupToRemove.UserId = 0;
            }
            else
            {
                Signups.Remove(signupToRemove);
            }
        }

        public bool IsFull()
        {
            var requiredSignups = Signups.Where(e => e.IsRequired);
            return requiredSignups.All(e => e.SignupFilled());
        }

        async public void UpdateSignupsOnMessageAsync(ISocketMessageChannel channel)
        {
            var messageRaw = await channel.GetMessageAsync(EventMessageId);
            var message = messageRaw as IUserMessage;

            if (message != null)
            {
                var messageStr = GetAnnouncementString();
                var sortedSignups = Signups.OrderBy(e => e.Role);

                foreach (var signup in sortedSignups)
                {
                    messageStr += $"{signup.Role.GetEmoji().Name}: ";
                    if (signup.SignupFilled())
                    {
                        messageStr += $"<@{signup.UserId}>";
                    }
                    messageStr += "\n";
                }

                messageStr += GetReactionInstructionsString();

                await message.ModifyAsync(x => { x.Content = messageStr; });
            }
        }
    }
}