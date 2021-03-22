using Discord;
using Microsoft.EntityFrameworkCore;

namespace CircusGroupsBot.Events
{
    [Owned]
    public class Signup
    {
        public string SignupId { get; set; }
        public Role Role { get; set; }
        public bool IsRequired { get; set; }
        //A userID of 0 means no one has signed up.
        public ulong UserId { get; set; }

        private Signup() { }

        public Signup(Role role, bool isRequired, ulong userId = 0)
        {
            this.Role = role;
            this.IsRequired = isRequired;
            this.UserId = userId;
        }

        public Signup(Role role, IUser user, bool isRequired) : this(role, isRequired, user.Id) { }

        public bool SignupFilled()
        {
            return UserId != 0;
        }
    }
}
