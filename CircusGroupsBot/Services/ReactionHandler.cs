using CircusGroupsBot.Events;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            Console.WriteLine("Init Reactions");
        }


        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> messageCacheable, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.UserId == client.CurrentUser.Id)
            {
                return Task.CompletedTask;
            }

            var messageId = messageCacheable.Id;
            var eventForMessage = DbContext.Events.Include(e => e.Signups).ThenInclude(e => e.Role).AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                var role = Role.GetRoleFromEmoji(reaction.Emote.Name);
                var signup = eventForMessage.Signups.FirstOrDefault(e => e.Role.RoleId == role.RoleId && e.UserId == reaction.UserId);
                if (signup != null)
                {
                    eventForMessage.Signups.Remove(signup);
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
            var eventForMessage = DbContext.Events.Include(e =>  e.Signups).ThenInclude(e => e.Role).AsQueryable().Where(e => e.EventMessageId == messageId).FirstOrDefault();
            if (eventForMessage != null)
            {
                var role = Role.GetRoleFromEmoji(reaction.Emote.Name);
                var dbRole = DbContext.Roles.AsQueryable().Where(e => e.RoleId == role.RoleId).FirstOrDefault();
                if (dbRole != null)
                {
                    eventForMessage.Signups.Add((new Signup(dbRole, false, reaction.UserId)));
                    eventForMessage.UpdateSignupsOnMessageAsync(channel);
                    DbContext.SaveChanges();
                    return channel.SendMessageAsync($"<@{reaction.UserId}> Signed up to {eventForMessage.EventName}");
                }

            }

            return Task.CompletedTask;
        }
    }
}
