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
                    eventForMessage.RemoveSignup(signup);
                    eventForMessage.UpdateSignupsOnMessageAsync(channel);
                    DbContext.SaveChanges();
                    return channel.SendMessageAsync($"<@{reaction.UserId}> is no longer joining {eventForMessage.EventName}");
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
                if (didAddSignup)
                {
                    eventForMessage.UpdateSignupsOnMessageAsync(channel);
                    DbContext.SaveChanges();
                    return channel.SendMessageAsync($"<@{reaction.UserId}> Signed up to {eventForMessage.EventName} as {role.GetEmoji()}");
                }
                else
                {
                    var getMsgTask = messageCacheable.GetOrDownloadAsync();
                    getMsgTask.ContinueWith(t => getMsgTask.Result.RemoveReactionAsync(reaction.Emote, reaction.UserId));
                    getMsgTask.ContinueWith(t => channel.SendMessageAsync($"Sorry <@{reaction.UserId}> we don't need any more {role.GetEmoji()}'s"));
                    return getMsgTask;
                }

            }

            return Task.CompletedTask;
        }
    }
}
