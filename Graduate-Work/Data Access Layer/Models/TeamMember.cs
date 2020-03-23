using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class TeamMember : BaseModel
    {
        public Employee Employee { get; set; }
        public Team Team { get; set; }
    }
}
