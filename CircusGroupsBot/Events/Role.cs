using Discord;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CircusGroupsBot.Events
{
    public enum Role
    {
        None,
        Tank,
        Healer,
        DD,
        Runner,
        Maybe
    };

    public static class RoleExtensions
    {
        public static string GetName(this Role role)
        {
            switch(role)
            {
                case Role.Tank:
                    return "Tank";
                case Role.Healer:
                    return "Healer";
                case Role.DD:
                    return "DD";
                case Role.Runner:
                    return "Runner";
                case Role.Maybe:
                    return "Maybe";
            }
            return "UnknownRole";
        }

        public static Emoji GetEmoji(this Role role)
        {
            switch (role)
            {
                case Role.Tank:
                    return new Emoji("🛡️");
                case Role.Healer:
                    return new Emoji("🚑");
                case Role.DD:
                    return new Emoji("⚔️");
                case Role.Runner:
                    return new Emoji("🏃");
                case Role.Maybe:
                    return new Emoji("❔");
            }
            return null;
        }

        public static Role EmojiToRole(string emoji)
        {
            switch(emoji)
            {
                case "🛡️":
                    return Role.Tank;
                case "🚑":
                    return Role.Healer;
                case "⚔️":
                    return Role.DD;
                case "🏃":
                    return Role.Runner;
                case "❔":
                    return Role.Maybe;
            }
            return Role.None;
        }
    }
}
