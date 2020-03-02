using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class Comment : BaseModel
    {
        public string Description { get; set; }
        public Employee Author { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}
