using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class ClearPinnedEventsModule : ModuleBase<SocketCommandContext>
    {
        private const int DefaultDays = 7;
        // TODO: Make this list more generic and implement a fuzzy check
        private List<string> TrialCategories = new List<string>() { "ps trials", "ps pledges and dungeons", "ps pvp", "pc eu", "pc trials", "pc pledges and dungeons", "pc pvp" };
        private readonly Logger Logger;
        private readonly DiscordSocketClient client;

        public ClearPinnedEventsModule(DiscordSocketClient client, Logger logger)
        {
            this.client = client;
            this.Logger = logger;
        }

        [Command("unpinevents")]
        [Summary("\"Unpin Events\" \"Days\"")]
        public async Task UnpinEventMessages(int days = DefaultDays)
        {
            await Logger.Log(new LogMessage(LogSeverity.Verbose, "ClearEvents", $"Clearing events older than {days} days old"));

            var user = Context.User;
            if (user == null)
            {
                return;
            }

            var category = ((SocketTextChannel)Context.Channel).Category;
            var categoryName = category.Name.ToLower();
            if (!TrialCategories.Contains(categoryName))
            {
                await user.SendMessageAsync($"You cannot unpin events in this category");
                return;
            }

            if (days < DefaultDays)
            {
                await user.SendMessageAsync($"You tried to unpin events less than than {DefaultDays} days old");
                return;
            }

            var botUser = client.CurrentUser;

            var pinnedMessages = await Context.Channel.GetPinnedMessagesAsync();
            var botPinnedMessages = pinnedMessages.Where(m => m.Author.IsBot).ToList();
            var agedPinnedMessages = botPinnedMessages.Where(pm => pm.CreatedAt < DateTimeOffset.UtcNow.AddDays(0 - days)).ToList();
            foreach(var pinnedMessage in agedPinnedMessages)
            {
                var message = (IUserMessage)pinnedMessage;
                await message.UnpinAsync();
            }

            await Logger.Log(new LogMessage(LogSeverity.Info, "Unpin Events", $"Events unpinned for {days} days by {user.Username}"));

            await user.SendMessageAsync($"Events older than {days} days unpinned ({agedPinnedMessages.Count} messages unpinned)");
            return;
        }
    }
}