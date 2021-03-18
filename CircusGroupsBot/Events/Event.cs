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
        public int EventId { get; set; }
        [Required]
        public ulong LeaderUserID { get; set; }
        [Required]
        public string EventName { get; set; }
        [Required]
        public string DateAndTime { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public List<Signup> Signups { get; set; }


        private Event(ulong leaderUserID, string eventName, string dateAndTime, string description = "")
        {
            this.LeaderUserID = leaderUserID;
            this.EventName = eventName;
            this.DateAndTime = dateAndTime;
            this.Description = description;
        }

        public Event(IUser leader, string eventName, string dateAndTime, string description = "",
    int tanks = 0, int healers = 0, int dds = 0, int runners = 0) : this(leader.Id, eventName, dateAndTime, description, tanks, healers, dds, runners) { }

        public Event(ulong leaderUserID, string eventName, string dateAndTime, string description = "",
    int tanks = 0, int healers = 0, int dds = 0, int runners = 0) : this(leaderUserID, eventName, dateAndTime, description)
        {
            for(int i = 0; i < tanks; ++i)
            {
                Signups.Add(new Signup(Role.Tank, true));
            }
            for(int i = 0; i < healers; ++i)
            {
                Signups.Add(new Signup(Role.Healer, true));
            }
            for (int i = 0; i < dds; ++i)
            {
                Signups.Add(new Signup(Role.DD, true));
            }
            for (int i = 0; i < runners; ++i)
            {
                Signups.Add(new Signup(Role.Runner, true));
            }
        }

        public Event(ulong leaderUserID, string eventName, string dateAndTime, List<Signup> signups, string description = "") : this(leaderUserID, eventName, dateAndTime, description)
        {
            this.Signups = signups;
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