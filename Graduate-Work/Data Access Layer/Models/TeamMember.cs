using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data_Access_Layer.Models
{
    [Table("TeamMembers")]
    public class TeamMember : BaseModel
    {
        public Employee Employee { get; set; }
        public int? EmployeeId { get; set; }
        public Project Project { get; set; }
        public int? ProjectId { get; set; }
        public Role Role { get; set; }
        public int? RoleId { get; set; }
    }
}
