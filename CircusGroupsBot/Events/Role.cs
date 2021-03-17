using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CircusGroupsBot.Events
{
    class Role
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string EmojiString { get; set; }

        public Role(string name, string emojiString)
        {
            this.Name = name;
            this.EmojiString = emojiString;
        }

        public Emoji GetEmoji()
        {
            return new Emoji(EmojiString);
        }

        [NotMapped]
        public static readonly IReadOnlyList<Role> AllRoles = new List<Role>()
        {
            new Role("Tank", "🛡️"),
            new Role("Healer", "🚑"),
            new Role("DD", "⚔️"),
            new Role("Runner", "🏃"),
            new Role("Maybe", "❔")
        };

    }
}
