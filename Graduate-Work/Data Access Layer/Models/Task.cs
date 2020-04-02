using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Task : BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string Severity { get; set; }
        public long Effort { get; set; }
        public long Recent { get; set; }
        public int TaskTypeId { get; set; }
        public int? EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public TaskType TaskType { get; set; }
        public Employee Employee { get; set; }
        public Project Project { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
