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
        public virtual Role Role { get; set; }
        public bool IsRequired { get; set; }

        public Signup() { }

        public Signup(Role role, bool isRequired)
        {
            this.Role = role;
            this.IsRequired = isRequired;
        }
    }
}
