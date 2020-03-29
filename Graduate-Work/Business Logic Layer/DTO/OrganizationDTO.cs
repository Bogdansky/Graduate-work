using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class OrganizationDTO : BaseModelDTO
    {
        public string Name { get; set; }
        public ICollection<ProjectDTO> Projects { get; set; }
        public ICollection<EmployeeDTO> Employees { get; set; }
    }
}
