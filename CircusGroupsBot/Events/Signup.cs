using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CircusGroupsBot.Events
{
    [Owned]
    public class Signup
    {
        public string SignupId { get; set; }
        public Role Role { get; set; }
        public bool IsRequired { get; set; }
        public ulong UserId { get; set; }

        private Signup() { }

        public Signup(Role role, bool isRequired, ulong userId = 0)
        {
            this.Role = role;
            this.IsRequired = isRequired;
            this.UserId = userId;
        }

        public Signup(Role role, IUser user, bool isRequired) : this(role, isRequired, user.Id) { }
    }
}
