using Business_Logic_Layer.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class TaskDTO : BaseModelDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string Severity { get; set; }
        public long Effort { get; set; }
        public long Recent { get; set; }
        public DateTime UpdateDate { get; set; }
        public TaskTypeEnum TaskType { get; set; }
        public TaskStatusEnum TaskStatus { get; set; }
        public int? EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public ICollection<CommentDTO> Comments { get; set; }
    }
}
