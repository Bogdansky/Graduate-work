using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Employee : BaseModel
    {
        public string FIO { get; set; }
        public DateTime Birthday { get; set; }
        public Role Role { get; set; }
        public int RoleId { get; set; }
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
