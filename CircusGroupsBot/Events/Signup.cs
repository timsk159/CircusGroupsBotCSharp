using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CircusGroupsBot.Events
{
    class Signup
    {
        [Key]
        public int Id { get; set; }
        public Role Role { get; set; }
        public bool IsRequired { get; set; }

        public Signup(Role role, bool isRequired)
        {
            this.Role = role;
            this.IsRequired = isRequired;
        }
    }
}
