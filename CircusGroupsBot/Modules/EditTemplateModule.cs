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
    public class EditTemplateModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;
        private readonly Logger Logger;

        public EditTemplateModule(CircusDbContext dbContext, Logger logger)
        {
            this.DbContext = dbContext;
            this.Logger = logger;
        }

        [Command("edittemplate")]
        [Summary("Edit an existing template")]
        public Task RunModule(string templateName, int tanks, int healers, int dds, int runners = 0)
        {
            Logger.Log(new LogMessage(LogSeverity.Verbose, "EditTemplate", $"Editing existing template {templateName}, {tanks}, {healers}, {dds}, {runners}"));

            var existingTemplate = DbContext.Templates.AsQueryable().FirstOrDefault(e => e.TemplateName == templateName);

            if(existingTemplate == null)
            {
                return ReplyAsync($"Template with name {templateName} does not exist");
            }

            existingTemplate.Tanks = tanks;
            existingTemplate.Healers = healers;
            existingTemplate.DDs = dds;
            existingTemplate.Runners = runners;

            DbContext.SaveChanges();

            return ReplyAsync($"Edited template named {templateName}");
        }
    }
}
