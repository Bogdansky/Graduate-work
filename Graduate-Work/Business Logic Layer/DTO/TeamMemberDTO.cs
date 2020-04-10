using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class TeamMemberDTO : BaseModelDTO
    {
        public EmployeeDTO Employee { get; set; }
        public ProjectDTO Project { get; set; }
        public int? EmployeeId { get; set; }
        public int? ProjectId { get; set; }
        public bool IsAdmin { get; set; }
        public RoleEnum Role { get; set; }
    }
}
