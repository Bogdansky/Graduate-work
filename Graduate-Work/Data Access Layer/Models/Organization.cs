using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Organization : BaseModel
    {
        public string Name { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}
