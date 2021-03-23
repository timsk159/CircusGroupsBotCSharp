using System.ComponentModel.DataAnnotations;

namespace CircusGroupsBot.Events
{
    public class Template
    {
        [Key]
        public int TemplateId { get; set; }

        public string TemplateName { get; set; }
        public int Tanks { get; set; }
        public int Healers { get; set; }
        public int DDs { get; set; }
        public int Runners { get; set; }

        public Template() { }

        public Template(string templateName, int tanks, int healers, int dDs, int runners)
        {
            TemplateName = templateName;
            Tanks = tanks;
            Healers = healers;
            DDs = dDs;
            Runners = runners;
        }
    }
}
