using Discord;
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
        public string EmojiString { get; set; }

        public Role(string name, string emojiString)
        {
            this.Name = name;
            this.EmojiString = emojiString;
        }

        public static readonly Role Tank = new Role("Tank", "🛡️");
        public static readonly Role Healer = new Role("Healer", "🚑");
        public static readonly Role DD = new Role("DD", "⚔️");
        public static readonly Role Runner = new Role("Runner", "🏃");
        public static readonly Role Maybe = new Role("Maybe", "❔");

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
