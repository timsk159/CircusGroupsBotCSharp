using CircusGroupsBot.Events;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CircusGroupsBot.Services
{
    class ReactionHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CircusDbContext DbContext;
        private readonly Logger logger;

        public ReactionHandler(CircusDbContext dbContext, DiscordSocketClient client, Logger logger)
        {
            this.client = client;
            this.DbContext = dbContext;
            this.logger = logger;
        }

        public void Init()
        {
            client.ReactionAdded += ReactionAddedAsync;
            client.ReactionRemoved += ReactionRemovedAsync;
        }


        private async Task ReactionRemovedAsync(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == client.CurrentUser.Id)
            {
                return;
            }

            var messageId = messageCacheable.Id;
            var eventForMessage = DbContext.Events.AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                //Must do this, as the message in the Cacheable is not available 90% of time.
                var message = await messageCacheable.GetOrDownloadAsync();

                var role = RoleExtensions.EmojiToRole(reaction.Emote.Name);
                var existingSignup = eventForMessage.Signups.FirstOrDefault(e => (e.Role == role || e.ReserveRole == role) && e.UserId == reaction.UserId);
                if (existingSignup != null)
                {
                    //If someone is a role-specific-reserve, we only remove their signup if they hit the role emoji, not the reserve emoji.
                    if(role == Role.Reserve && existingSignup.ReserveRole != Role.None)
                    {
                        return;
                    }

                    bool wasFull = eventForMessage.IsFull();
                    bool roleWasFull = eventForMessage.IsRoleFull(role);

                    eventForMessage.RemoveSignup(existingSignup);
                    DbContext.SaveChanges();

                    eventForMessage.UpdateSignupsOnMessageAsync(message);

                    var user = await channel.GetUserAsync(reaction.UserId);
                    if (user != null)
                    {
                        var returnTask = user.SendMessageAsync($"You are no longer joining {eventForMessage.EventName}");

                        Signup nextReserve = null;

                        if (wasFull && !eventForMessage.IsFull())
                        {
                            var leaderMsg = $"Your event {eventForMessage.EventName} is no longer full, as {user.Mention} is no longer joining";
                            nextReserve = eventForMessage.GetNextReserve();
                            if (nextReserve != null)
                            {
                                leaderMsg += $"\nAs <@{nextReserve.UserId}> is the first reserve, they have been notified";
                            }

                            var leaderUser = await channel.GetUserAsync(eventForMessage.LeaderUserID);
                            await returnTask.ContinueWith(t => leaderUser.SendMessageAsync(leaderMsg));
                        }
                        else if(roleWasFull && !eventForMessage.IsRoleFull(role))
                        {
                            nextReserve = eventForMessage.GetNextReserve(role);
                        }

                        if (nextReserve != null)
                        {
                            var reserveUser = await channel.GetUserAsync(nextReserve.UserId);
                            await returnTask.ContinueWith(t => reserveUser.SendMessageAsync($"A spot has opened up in {eventForMessage.EventName} and you are next on the reserves!"));
                        }

                        await returnTask;
                    }
                }
            }

            return;
        }

        private async Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == client.CurrentUser.Id)
            {
                return;
            }

            //Must do this, as the message in the Cacheable is not available 90% of time.
            var message = await messageCacheable.GetOrDownloadAsync();

            var messageId = messageCacheable.Id;
            var eventForMessage = DbContext.Events.AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                //Remove people adding emojies we don't recognise
                var role = RoleExtensions.EmojiToRole(reaction.Emote.Name);
                if (role == Role.None)
                {
                    var msgTask = message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    return;
                }

                //This is to handle people that were moved into reserve due to an event being modified
                if (role == Role.Reserve && eventForMessage.Signups.Any(e => e.Role == Role.Reserve && e.UserId == reaction.UserId))
                {
                    return;
                }
                bool wasFull = eventForMessage.IsFull();
                var didAddSignup = eventForMessage.TryAddSignup(role, reaction.UserId);

                var user = await channel.GetUserAsync(reaction.UserId);
                if (didAddSignup)
                {
                    DbContext.SaveChanges();

                    eventForMessage.UpdateSignupsOnMessageAsync(message);

                    var msgTask = user.SendMessageAsync($"You successfully signed up to {eventForMessage.EventName} as {role.GetEmoji()}. Have fun!");

                    if (eventForMessage.IsFull() && !wasFull)
                    {
                        var leaderUser = await channel.GetUserAsync(eventForMessage.LeaderUserID);
                        await msgTask.ContinueWith(t => leaderUser.SendMessageAsync($"Your event {eventForMessage.EventName} is now full!"));
                    }

                    await msgTask;
                }
                else
                {
                    var signup = new Signup(Role.Reserve, role, false, DateTime.Now, reaction.UserId);
                    eventForMessage.TryAddSignup(signup);
                    DbContext.SaveChanges();
                    eventForMessage.UpdateSignupsOnMessageAsync(message);

                    await user.SendMessageAsync($"Sorry, you were placed in reserve for {eventForMessage.EventName} because we don't need any more {role.GetEmoji()}'s");
                }
            }

            return;
        }
    }
}
