using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class TaskType : BaseModel
    {
        public string Name { get; set; }
        public ICollection<Task> Tasks { get; set; }
        public TaskType()
        {
            Tasks = new HashSet<Task>();
        }
    }
}
