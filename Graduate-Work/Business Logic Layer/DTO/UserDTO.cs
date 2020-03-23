using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class UserDTO : BaseModelDTO
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
