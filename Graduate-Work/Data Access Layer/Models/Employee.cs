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
        public int? UserId { get; set; }
        public int? OrganizationId { get; set; }
        public virtual User User { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual ICollection<TeamMember> Projects { get; set; }
    }
}
