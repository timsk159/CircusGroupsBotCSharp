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
                var message = await messageCacheable.GetOrDownloadAsync();
                var role = RoleExtensions.EmojiToRole(reaction.Emote.Name);
                var signup = eventForMessage.Signups.FirstOrDefault(e => e.Role == role && e.UserId == reaction.UserId);
                if (signup != null)
                {
                    bool wasFull = eventForMessage.IsFull();
                    eventForMessage.RemoveSignup(signup);
                    eventForMessage.UpdateSignupsOnMessageAsync(message);
                    DbContext.SaveChanges();

                    var user = await channel.GetUserAsync(reaction.UserId);

                    if (user != null)
                    {
                        var returnTask = user.SendMessageAsync($"You are no longer joining {eventForMessage.EventName}");
                        if(wasFull)
                        {
                            var leaderUser = await channel.GetUserAsync(eventForMessage.LeaderUserID);
                            await returnTask.ContinueWith(t => leaderUser.SendMessageAsync($"Your event {eventForMessage.EventName} is no longer full, as {user.Username} is no longer joining"));
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

            var role = RoleExtensions.EmojiToRole(reaction.Emote.Name);
            var message = await messageCacheable.GetOrDownloadAsync();

            if (role == Role.None)
            {
                var msgTask = message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                return;
            }

            var messageId = messageCacheable.Id;
            var eventForMessage = DbContext.Events.AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                var didAddSignup = eventForMessage.TryAddSignup(role, reaction.UserId);

                var user = await channel.GetUserAsync(reaction.UserId);


                if (didAddSignup)
                {
                    eventForMessage.UpdateSignupsOnMessageAsync(message);
                    DbContext.SaveChanges();
                    var msgTask = user.SendMessageAsync($"You successfully signed up to {eventForMessage.EventName} as {role.GetEmoji()}. Have fun!");

                    if (eventForMessage.IsFull())
                    {
                        var leaderUser = await channel.GetUserAsync(eventForMessage.LeaderUserID);
                        await msgTask.ContinueWith(t => leaderUser.SendMessageAsync($"Your event {eventForMessage.EventName} is now full!"));
                    }

                    await msgTask;
                }
                else
                {
                    var msgTask = message.RemoveReactionAsync(reaction.Emote, reaction.UserId);
                    await msgTask.ContinueWith(t => user.SendMessageAsync($"Sorry, you were not signed up to {eventForMessage.EventName} because we don't need any more {role.GetEmoji()}'s"));
                    await msgTask;
                }
            }

            return;
        }
    }
}
