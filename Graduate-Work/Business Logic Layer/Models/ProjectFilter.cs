using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Models
{
    public class ProjectFilter
    {
        public string Like { get; set; }
        public int? ProjectId { get; set; }
        public int? EmployeeId { get; set; }
        public ProjectFilterEnum Filter { get; set; }
    }

    public enum ProjectFilterEnum
    {
        All, Mine
    }
}
