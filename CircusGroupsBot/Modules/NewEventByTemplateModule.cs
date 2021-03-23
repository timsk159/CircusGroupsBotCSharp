using CircusGroupsBot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircusGroupsBot.Modules
{
    class NewEventByTemplateModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;
        public NewEventByTemplateModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }


    }
}
