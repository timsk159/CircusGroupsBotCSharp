using CircusGroupsBot.Events;
using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class NewTemplateModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;

        public NewTemplateModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }

        [Command("newtemplate")]
        [Summary("\"Template Name\" 1 1 1 1")]
        public Task RunModule(string templateName, int tanks, int healers, int dds, int runners = 0)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "NewTemplate", $"Creating new template {templateName}, {tanks}, {healers}, {dds}, {runners}"));

            if(DbContext.Templates.AsQueryable().Any(e => e.TemplateName == templateName))
            {
                return ReplyAsync($"Template already exists with name {templateName}, did you mean to use $EditTemplate?");
            }

            var newTemplate = new Template(templateName, tanks, healers, dds, runners);

            DbContext.Templates.Add(newTemplate);
            DbContext.SaveChanges();

            return ReplyAsync($"Created new template named {templateName}");
        }
    }
}
