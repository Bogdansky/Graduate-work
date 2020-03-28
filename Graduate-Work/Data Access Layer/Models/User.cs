using System;
using System.Collections.Generic;
using System.Text;

namespace Data_Access_Layer.Models
{
    public class User : BaseModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
    }
}
