using Discord;
using Microsoft.EntityFrameworkCore;
using System;

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
        public DateTime SignupDate { get; set; }

        private Signup() { }

        public Signup(Role role, bool isRequired, DateTime signupDate, ulong userId = 0)
        {
            this.Role = role;
            this.IsRequired = isRequired;
            this.UserId = userId;
            this.SignupDate = signupDate;
        }

        public Signup(Role role, IUser user, DateTime signupDate, bool isRequired) : this(role, isRequired, signupDate, user.Id) { }

        public bool SignupFilled()
        {
            return UserId != 0;
        }
    }
}