using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Services
{
    class MessageModificationHandler
    {
        private readonly DiscordSocketClient client;
        private readonly CircusDbContext DbContext;
        private readonly CommandHandler commandHandler;
        private readonly Logger logger;

        public MessageModificationHandler(CircusDbContext dbContext, DiscordSocketClient client, Logger logger, CommandHandler commandHandler)
        {
            this.client = client;
            this.DbContext = dbContext;
            this.logger = logger;
            this.commandHandler = commandHandler;
        }

        public void Init()
        {
            client.MessageDeleted += Client_MessageDeletedAsync;
            client.MessageUpdated += Client_MessageUpdated;
        }

        private async Task Client_MessageDeletedAsync(Cacheable<IMessage, ulong> messageCacheable, ISocketMessageChannel channel)
        {
            var eventForMessage = DbContext.Events.AsQueryable().Where(e => e.CommandMessageId == messageCacheable.Id).FirstOrDefault();
            if(eventForMessage != null)
            {
                var messageForEvent = await channel.GetMessageAsync(eventForMessage.EventMessageId);
                await channel.DeleteMessageAsync(messageForEvent);

                DbContext.Events.Remove(eventForMessage);
                DbContext.SaveChanges();
            }
        }

        private Task Client_MessageUpdated(Cacheable<IMessage, ulong> messageCacheable, SocketMessage message, ISocketMessageChannel channel)
        {
            return Task.CompletedTask;
        }
    }
}
