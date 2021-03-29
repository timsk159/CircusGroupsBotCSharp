using System.Collections.Generic;

namespace CircusGroupsBot.Models
{
    public class Command
    {
        public string Alias { get; set; }
        public List<string> Commands { get; set; }
    }
}