using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class TrackingHistoryDTO : BaseModelDTO
    {
        public int TaskId { get; set; }
        public int EmployeeId { get; set; }
        public long TrackedTime { get; set; }
        public DateTime Date { get; set; }
        public TaskDTO Task { get; set; }
        public EmployeeDTO Employee { get; set; }
    }
}
