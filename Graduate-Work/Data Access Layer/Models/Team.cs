using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Team : BaseModel
    {
        public Project Project { get; set; }
        public ICollection<TeamMember> Employees { get; set; }
    }
}
