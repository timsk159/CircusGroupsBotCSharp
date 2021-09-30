using System;
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
        public ulong CommandMessageId { get; set; }
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
            var time = DateTime.UtcNow;

            for (int i = 0; i < tanks; ++i)
            {
                Signups.Add(new Signup(Role.Tank, true, time));
            }
            for (int i = 0; i < healers; ++i)
            {
                Signups.Add(new Signup(Role.Healer, true, time));
            }
            for (int i = 0; i < dds; ++i)
            {
                Signups.Add(new Signup(Role.DD, true, time));
            }
            for (int i = 0; i < runners; ++i)
            {
                Signups.Add(new Signup(Role.Runner, true, time));
            }
        }

        public Event(ulong leaderUserID, string eventName, string dateAndTime, ulong eventMessageId, List<Signup> signups, string description = "") : this(leaderUserID, eventName, dateAndTime, eventMessageId, description)
        {
            this.Signups = signups;
        }

        public string GetAnnouncementString()
        {
            return $@"{EventName}
Scheduled For: {DateAndTime}

Leader: <@{LeaderUserID}>

{Description}

";
        }

        public string GetReactionInstructionsString()
        {
            var returnStr = "\nReact With:\n";
            var hasRequiredRoles = Signups.Any(e => e.IsRequired);

            foreach (Role val in Enum.GetValues(typeof(Role)))
            {
                if (val == Role.None)
                {
                    continue;
                }

                bool shouldAddRole = false;

                if (val == Role.Maybe || val == Role.Reserve)
                {
                    shouldAddRole = true;
                }
                else
                {
                    if (hasRequiredRoles)
                    {
                        if (Signups.Any(e => e.Role == val))
                        {
                            shouldAddRole = true;
                        }
                    }
                    else
                    {
                        if (val != Role.Runner)
                        {
                            shouldAddRole = true;
                        }
                    }
                }
                if (shouldAddRole)
                {
                    returnStr += $"{val.GetEmoji().Name} to sign up as {val.GetName()}\n";
                }
            }
            return returnStr;
        }

        public async void AddReactionsToMessageAsync(IUserMessage message)
        {
            var allRoleReactionsEmoji = new List<Emoji>();
            var allRoles = Enum.GetValues(typeof(Role)).OfType<Role>();

            if (Signups.Any(e => e.IsRequired))
            {
                foreach (var role in allRoles)
                {
                    if (role == Role.None)
                    {
                        continue;
                    }
                    if (Signups.Any(e => e.Role == role))
                    {
                        allRoleReactionsEmoji.Add(role.GetEmoji());
                    }
                }
                allRoleReactionsEmoji.Add(Role.Maybe.GetEmoji());
                allRoleReactionsEmoji.Add(Role.Reserve.GetEmoji());
            }
            else
            {
                foreach (var role in allRoles)
                {
                    if (role == Role.None)
                    {
                        continue;
                    }
                    if (role != Role.Runner)
                    {
                        allRoleReactionsEmoji.Add(role.GetEmoji());
                    }
                }
            }
            await message.AddReactionsAsync(allRoleReactionsEmoji.ToArray());
        }

        public bool TryAddSignup(Role role, ulong userID)
        {
            return TryAddSignup(new Signup(role, false, DateTime.UtcNow, userID));
        }

        public bool TryAddSignup(Signup signup)
        {
            if (signup.Role != Role.Maybe && signup.Role != Role.Reserve && Signups.Any(e => e.IsRequired == true))
            {
                var freeSlot = Signups.FirstOrDefault(e => e.Role == signup.Role && !e.IsFilled());
                if (freeSlot != null)
                {
                    freeSlot.UserId = signup.UserId;
                    return true;
                }
                return false;
            }
            else
            {
                Signups.Add(signup);
                return true;
            }
        }

        public void RemoveSignup(Role role, ulong userID)
        {
            RemoveSignup(Signups.FirstOrDefault(e => e.Role == role && e.UserId == userID));
        }

        public void RemoveSignup(Signup signupToRemove)
        {
            if (signupToRemove == null)
            {
                return;
            }
            if (signupToRemove.IsRequired == true)
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
            return requiredSignups.Any() && requiredSignups.All(e => e.IsFilled());
        }

        public bool IsRoleFull(Role role)
        {
            var requiredSignups = Signups.Where(e => e.IsRequired);
            if(!requiredSignups.Any())
            {
                return false;
            }
            return requiredSignups.Any(e => !e.IsFilled());
        }

        public Signup GetNextReserve(Role priorityRole = Role.None)
        {
            Signup nextReserve = null;

            if (priorityRole != Role.None)
            {
                nextReserve = Signups.FirstOrDefault(e => e.Role == Role.Reserve && e.ReserveRole == priorityRole);
            }
            if(nextReserve == null)
            {
                nextReserve = Signups.FirstOrDefault(e => e.Role == Role.Reserve);
            }

            return nextReserve;
        }

        //TODO: This is written in a super stupid way. Needs re-writing :(
        public void TransferSignups(Event newEvent, out List<Signup> newReserves)
        {
            newReserves = new List<Signup>();
            if (newEvent.Signups.All(e => !e.IsRequired))
            {
                foreach (var signup in Signups)
                {
                    if (signup.IsFilled())
                    {
                        newEvent.Signups.Add(signup);
                    }
                }
                Signups = newEvent.Signups;
                return;
            }

            foreach (var signup in Signups)
            {
                if (!signup.IsFilled())
                {
                    continue;
                }

                if (signup.Role == Role.Maybe || signup.Role == Role.Reserve)
                {
                    //Don't add the same reserve or maybe more than once (if someone signed up in more than 1 role)
                    if (newEvent.Signups.Any(e => e.Role == signup.Role && e.UserId == signup.UserId))
                    {
                        continue;
                    }
                    newEvent.Signups.Add(new Signup(signup.Role, false, DateTime.UtcNow, signup.UserId));
                    continue;
                }

                var spotInNewEvent = newEvent.Signups.FirstOrDefault(e => e.Role == signup.Role && e.IsRequired && !e.IsFilled());
                if (spotInNewEvent != null)
                {
                    spotInNewEvent.UserId = signup.UserId;
                }
                else
                {
                    //Don't add the same reserve or maybe more than once (if someone signed up in more than 1 role)
                    if (newEvent.Signups.Any(e => e.Role == Role.Reserve && e.UserId == signup.UserId))
                    {
                        continue;
                    }
                    var newReserve = new Signup(Role.Reserve, false, DateTime.UtcNow, signup.UserId);
                    newEvent.Signups.Add(newReserve);
                    newReserves.Add(newReserve);
                }
            }
            Signups = newEvent.Signups;
        }

        async public void UpdateSignupsOnMessageAsync(IUserMessage message, IGuild Guild)
        {
            if (message != null)
            {
                var messageStr = "";
                var sortedSignups = Signups.OrderBy(e => e.Role);

                foreach (var signup in sortedSignups)
                {
                    if(signup.ReserveRole != Role.None)
                    {
                        messageStr += $"{signup.Role.GetEmoji().Name}{signup.ReserveRole.GetEmoji().Name}: ";
                    }
                    else
                    {
                        messageStr += $"{signup.Role.GetEmoji().Name}: ";
                    }
                    if (signup.IsFilled())
                    {
                        var user = await Guild.GetUserAsync(signup.UserId);
                        if (user != null)
                        {
                            messageStr += $"{user.Nickname}";
                        }
                        else
                        {
                            messageStr += $"Unknown User";
                        }
                    }
                    messageStr += "\n";
                }


                var embed = message.Embeds.FirstOrDefault();
                if (embed != null)
                {
                    var eb = embed.ToEmbedBuilder();

                    var signupsField = eb.Fields.FirstOrDefault(e => e.Name == "Signups");
                    if (signupsField != null)
                    {
                        if (string.IsNullOrEmpty(messageStr))
                        {
                            eb.Fields.Remove(signupsField);
                        }
                        else
                        {
                            signupsField.Value = messageStr;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(messageStr))
                        {
                            eb.AddField("Signups", messageStr);
                        }
                    }

                    await message.ModifyAsync(x => { x.Embed = eb.Build(); });
                }
            }
        }
    }
}