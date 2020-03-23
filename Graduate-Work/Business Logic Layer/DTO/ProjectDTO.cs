using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class ProjectDTO : BaseModelDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public object Team { get; set; }
        public ICollection<TaskDTO> Tasks { get; set; }
        public ICollection<EmployeeDTO> Administartors { get; set; }
    }
}
