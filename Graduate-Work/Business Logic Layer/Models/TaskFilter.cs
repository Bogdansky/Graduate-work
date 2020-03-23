using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Models
{
    public class TaskFilter
    {
        public int TaskId { get; set; }
        public TaskFilterTypes TaskFilterType { get; set; }
        public int? EmployeeId { get; set; } 
        public int? ProjectId { get; set; }
    }

    public enum TaskFilterTypes
    {
        All, AllMine, MineInProject
    }
}
