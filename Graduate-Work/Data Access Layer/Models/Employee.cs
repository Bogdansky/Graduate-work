using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Employee : BaseModel
    {
        public string FullName { get; set; }
        public DateTime Birthday { get; set; }
        public Role Role { get; set; }
        public int RoleId { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
