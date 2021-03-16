using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Discord;

namespace CircusGroupsBot.Events
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public ulong LeaderUserID { get; set; }
        [Required]
        public string EventName { get; set; }
        [Required]
        public string DateAndTime { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Tanks { get; set; }
        [Required]
        public int Healers { get; set; }
        [Required]
        public int Dds { get; set; }
        [Required]
        public int Runners { get; set; }

        public Event(IUser leader, string eventName, string dateAndTime, string description = "",
            int tanks = 0, int healers = 0, int dds = 0, int runners = 0) : this(leader.Id, eventName, dateAndTime, description, tanks, healers, dds, runners) { }

        public Event(ulong leaderUserID, string eventName, string dateAndTime, string description = "",
    int tanks = 0, int healers = 0, int dds = 0, int runners = 0)
        {
            this.LeaderUserID = leaderUserID;
            this.EventName = eventName;
            this.DateAndTime = dateAndTime;
            this.Description = description;
            this.Tanks = tanks;
            this.Healers = healers;
            this.Dds = dds;
            this.Runners = runners;
        }

        public string GetAnnouncementString()
        {
            return $@"@everyone
{EventName}
Scheduled For: {DateAndTime}

Leader: <@{LeaderUserID}>
{Description}
";
        }
    }
}