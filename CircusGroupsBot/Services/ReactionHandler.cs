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
                eventForMessage.Signups.Add((new Signup(Role.Tank, false)));
                DbContext.SaveChanges();
                return channel.SendMessageAsync("Signed up!");
            }

            return Task.CompletedTask;
        }
    }
}
