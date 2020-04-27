using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Models
{
    public class TrackedTask
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public long Effort { get; set; }
        public long PreviousRecent { get; set; }
        public long NextRecent { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
