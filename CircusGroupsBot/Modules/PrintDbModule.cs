using CircusGroupsBot.Events;
using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class PrintDbModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;
        public PrintDbModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }

        [Command("PrintDb")]
        [Summary("Logs the contents of DB. NOTE: FOR DEV USE ONLY")]
        public Task RunModuleAsync(string eventName, string dateandtime, string description = "", int tanks = 0, int healers = 0, int dds = 0)
        {
            var events = DbContext.Events;

            foreach(var _event in events)
            {
                Logger.Log(new LogMessage(LogSeverity.Verbose, "PrintDb", _event.EventName));
            }

            return ReplyAsync("Check logs");
        }
    }
}
