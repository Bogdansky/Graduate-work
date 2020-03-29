using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Project : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Organization Organization { get; set; }
        public int? OrganizationId { get; set; }
        public ICollection<TeamMember> Team { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public ICollection<Employee> Administrators { get; set; }
    }
}
