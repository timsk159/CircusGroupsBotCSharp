using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CircusGroupsBot.Events
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public string EmojiString { get; set; }

        private Role(int roleId, string name)
        {
            this.RoleId = roleId;
            this.Name = name;
        }

        private Role(int roleId, string name, string emojiString)
        {
            this.Name = name;
            this.EmojiString = emojiString;
            this.RoleId = roleId;
        }

        public static readonly Role Tank = new Role(1, "Tank", "🛡️");
        public static readonly Role Healer = new Role(2, "Healer", "🚑");
        public static readonly Role DD = new Role(3, "DD", "⚔️");
        public static readonly Role Runner = new Role(4, "Runner", "🏃");
        public static readonly Role Maybe = new Role(5, "Maybe", "❔");

        [NotMapped]
        public static readonly IReadOnlyList<Role> AllRoles = new List<Role>()
        {
            Tank, Healer, DD, Runner, Maybe
        };

        public Emoji GetEmoji()
        {
            return new Emoji(EmojiString);
        }
    }
}
