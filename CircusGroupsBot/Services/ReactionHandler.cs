using CircusGroupsBot.Events;
using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CircusGroupsBot.Services
{
    class ReactionHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CircusDbContext DbContext;

        public ReactionHandler(CircusDbContext dbContext, DiscordSocketClient client)
        {
            this.client = client;
            this.DbContext = dbContext;
        }

        public void Init()
        {
            client.ReactionAdded += ReactionAdded;
            client.ReactionRemoved += ReactionRemoved;
        }


        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == client.CurrentUser.Id)
            {
                return Task.CompletedTask;
            }

            var messageId = messageCacheable.Id;
            var eventForMessage = DbContext.Events.AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                var role = RoleExtensions.EmojiToRole(reaction.Emote.Name);
                var signup = eventForMessage.Signups.FirstOrDefault(e => e.Role == role && e.UserId == reaction.UserId);
                if (signup != null)
                {
                    bool wasFull = eventForMessage.IsFull();
                    eventForMessage.RemoveSignup(signup);
                    eventForMessage.UpdateSignupsOnMessageAsync(channel);
                    DbContext.SaveChanges();

                    var user = reaction.User.Value;

                    if (user != null)
                    {
                        var returnTask = user.SendMessageAsync($"You are no longer joining {eventForMessage.EventName}");
                        if(wasFull)
                        {
                            var leaderUser = client.GetUser(eventForMessage.LeaderUserID);
                            returnTask.ContinueWith(t => leaderUser.SendMessageAsync($"Your event {eventForMessage.EventName} is no longer full, as {user.Username} is no longer joining"));
                        }
                        return returnTask;
                    }
                }
            }

            return Task.CompletedTask;
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == client.CurrentUser.Id)
            {
                return Task.CompletedTask;
            }
            
            var messageId = messageCacheable.Id;
            var eventForMessage = DbContext.Events.AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                var role = RoleExtensions.EmojiToRole(reaction.Emote.Name);
                var didAddSignup = eventForMessage.TryAddSignup(role, reaction.UserId);

                var user = reaction.User.Value;
                if (user != null)
                {
                    if (didAddSignup)
                    {
                        eventForMessage.UpdateSignupsOnMessageAsync(channel);
                        DbContext.SaveChanges();
                        var msgTask = user.SendMessageAsync($"You successfully signed up to {eventForMessage.EventName} as {role.GetEmoji()}. Have fun!");

                        if (eventForMessage.IsFull())
                        {
                            var leaderUser = client.GetUser(eventForMessage.LeaderUserID);
                            msgTask.ContinueWith(t => leaderUser.SendMessageAsync($"Your event {eventForMessage.EventName} is now full!"));
                        }

                        return msgTask;
                    }
                    else
                    {
                        var getMsgTask = messageCacheable.GetOrDownloadAsync();
                        getMsgTask.ContinueWith(t => getMsgTask.Result.RemoveReactionAsync(reaction.Emote, reaction.UserId));
                        getMsgTask.ContinueWith(t => user.SendMessageAsync($"Sorry, you were not signed up to {eventForMessage.EventName} because we don't need any more {role.GetEmoji()}'s"));
                        return getMsgTask;
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
