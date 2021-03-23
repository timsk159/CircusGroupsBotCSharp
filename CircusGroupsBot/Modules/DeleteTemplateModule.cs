using CircusGroupsBot.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class DeleteTemplateModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;

        public DeleteTemplateModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }

        [Command("deletetemplate")]
        [Summary("Delete a pre-created template")]
        public Task RunModule(string templateName)
        {
            var template = DbContext.Templates.FirstOrDefault(e => e.TemplateName == templateName);
            if (template == null)
            {
                return ReplyAsync($"No template found with name {templateName}");
            }

            DbContext.Templates.Remove(template);
            DbContext.SaveChanges();

            return ReplyAsync($"Deleted template with name {templateName}");
        }
    }
}
