using CircusGroupsBot.Events;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircusGroupsBot.Services
{
    public class CircusDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Role> Roles { get; set; }

        public CircusDbContext(DbContextOptions<CircusDbContext> options) : base(options)
        {
        }
    }
}
