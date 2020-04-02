using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class CommentDTO : BaseModelDTO
    {
        public string Description { get; set; }
        public EmployeeDTO Author { get; set; }
        public int? TaskId { get; set; }
    }
}
