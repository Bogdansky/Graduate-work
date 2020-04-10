using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class TaskStatus : BaseModel
    {
        public string Name { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
