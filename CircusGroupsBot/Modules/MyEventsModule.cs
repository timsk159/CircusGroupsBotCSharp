using CircusGroupsBot.Services;
using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircusGroupsBot.Modules
{
    public class MyEventsModule : ModuleBase<SocketCommandContext>
    {
        private readonly CircusDbContext DbContext;

        public MyEventsModule(CircusDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        [Command("myevents")]
        [Summary("You can optionally choose how far back in time to check. The default is 7 days:\n$myevents 5")]
        public Task RunModuleAsync(int days = 7)
        {
            var user = Context.User;
            var signupDate = DateTime.UtcNow.AddDays(-days);
            if (user == null)
            {
                return null;
            }

            var request = DbContext.Events.AsQueryable().Where(e => e.Signups.Where(s => s.UserId == user.Id && s.SignupDate > signupDate).Count() > 0).ToList();

            var eb = new EmbedBuilder();
            eb.WithTitle("Your Events");
            eb.WithDescription($"The following are the events you signed up to in the last {days} days");
            eb.Color = Color.Gold;
            var sb = new StringBuilder();
            if(request.Count == 0)
            {
                sb.Append($"You have not signed up to any events in the last {days} days");
            }

            foreach (var @event in request)
            {
                var role = @event.Signups.Where(e => e.UserId == user.Id).FirstOrDefault().Role.ToString();
                sb.Append(Environment.NewLine);
                sb.Append($"{@event.EventName}: {@event.DateAndTime} as {role}");
            }

            eb.AddField("Signups", sb.ToString());

            return user.SendMessageAsync(embed: eb.Build());
        }
    }
}