using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;

namespace CircusGroupsBot.Events
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }

        private Role(int roleId, string name)
        {
            this.RoleId = roleId;
            this.Name = name;
        }

        public static readonly Role Tank = new Role(1, "Tank");
        public static readonly Role Healer = new Role(2, "Healer");
        public static readonly Role DD = new Role(3, "DD");
        public static readonly Role Runner = new Role(4, "Runner");
        public static readonly Role Maybe = new Role(5, "Maybe");

        [NotMapped]
        public static readonly IReadOnlyList<Role> AllRoles = new List<Role>()
        {
            Tank, Healer, DD, Runner, Maybe
        };

        public Emoji GetEmoji()
        {
            //TODO: This is a workaround, because I can't figure out how to store an emoji in the DB
            switch(RoleId)
            {
                case 1:
                    return new Emoji("🛡️");
                case 2:
                    return new Emoji("🚑");
                case 3:
                    return new Emoji("⚔️");
                case 4:
                    return new Emoji("🏃");
                case 5:
                    return new Emoji("❔");
            }
            return null;
        }

        public static Role GetRoleFromEmoji(string emoji)
        {
            return AllRoles.FirstOrDefault(e => e.GetEmoji().Name == emoji);
        }
    }
}
