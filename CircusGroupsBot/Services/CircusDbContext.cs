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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Without this, I get a null reference exception when creating a migration.
            //Guessing cos the static vars are not initialized at design time?
            foreach(var role in Role.AllRoles)
            {
                Console.WriteLine($"Role! {role.Name}");
            }
            modelBuilder.Entity<Role>().HasData(Role.AllRoles);
            base.OnModelCreating(modelBuilder);
        }
    }
}
