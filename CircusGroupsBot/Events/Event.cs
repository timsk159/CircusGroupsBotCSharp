using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Discord;

namespace CircusGroupsBot.Events
{
    public class Event
    {
        [Required]
        public IUser Leader { get; private set; }
        [Required]
        public string EventName { get; private set; }
        [Required]
        public string DateAndTime { get; private set; }
        [Required]
        public string Description { get; private set; }
        [Required]
        public int Tanks { get; private set; }
        [Required]
        public int Healers { get; private set; }
        [Required]
        public int DDs { get; private set; }
        [Required]
        public int Runners { get; private set; }

        public Event(IUser leader, string eventName, string dateandtime, string description = "", 
            int tanks = 0, int healers = 0, int dds = 0, int runners = 0)
        {
            this.Leader = leader;
            this.EventName = eventName;
            this.DateAndTime = dateandtime;
            this.Description = description;
            this.Tanks = tanks;
            this.Healers = healers;
            this.DDs = dds;
            this.Runners = runners;
        }

        public string GetAnnouncementString()
        {
            return $@"@everyone
{EventName}
Scheduled For: {DateAndTime}

Leader: {Leader.Mention}
{Description}
";
        }
    }
}