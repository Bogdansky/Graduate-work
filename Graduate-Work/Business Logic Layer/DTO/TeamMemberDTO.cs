using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class TeamMemberDTO : BaseModelDTO
    {
        public int? EmployeeId { get; set; }
        public int? ProjectId { get; set; }
        public string Role { get; set; }
    }
}
