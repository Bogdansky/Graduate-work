using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Role : BaseModel
    {
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<TeamMember> TeamMembers { get; set; }
        public Role()
        {
            Employees = new HashSet<Employee>();
            TeamMembers = new HashSet<TeamMember>();
        }
    }
}
