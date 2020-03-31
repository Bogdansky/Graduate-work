using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Models
{
    public class TaskFilter
    {
        public TaskFilterTypes TaskFilterType { get; set; }
        public int? EmployeeId { get; set; } 
        public int? ProjectId { get; set; }
    }

    public enum TaskFilterTypes
    {
        AllMine, AllInProject, MineInProject
    }
}
