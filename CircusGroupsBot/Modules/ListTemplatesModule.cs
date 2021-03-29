using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class ListTemplatesModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;

        public ListTemplatesModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }

        [Command("listtemplates")]
        [Summary("Will send you a message with details of all existing templates")]
        public Task RunModule()
        {
            var user = Context.User;
            if (user == null)
            {
                return null;
            }

            var templates = DbContext.Templates;

            var sb = new StringBuilder();

            sb.Append("Template List:\n");

            foreach(var template in templates)
            {
                sb.Append($"{template.TemplateName} Tanks: {template.Tanks} Healers: {template.Healers} DDs: {template.DDs} Runners: {template.Runners}");
                sb.Append("\n");
            }

            return user.SendMessageAsync(sb.ToString());

        }
    }
}
