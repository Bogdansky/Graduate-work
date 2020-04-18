using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Models
{
    public class EmployeeFilter
    {
        public int MinRate { get; set; }
        public int MaxRate { get; set; }
        public int MinProjectNumber { get; set; }
        public int MaxProjectNumber { get; set; }
        public int ProjectId { get; set; }
        public RoleEnum Role { get; set; }
    }
}
