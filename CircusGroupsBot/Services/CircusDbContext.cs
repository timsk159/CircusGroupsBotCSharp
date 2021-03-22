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

        public CircusDbContext(DbContextOptions<CircusDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>().OwnsMany(e => e.Signups, a =>
            {
                a.WithOwner().HasForeignKey("EventId");
                a.Property<string>("SignupId").ValueGeneratedOnAdd();
                a.HasKey("SignupId");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
