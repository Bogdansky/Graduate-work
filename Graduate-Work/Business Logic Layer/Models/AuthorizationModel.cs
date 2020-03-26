using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Models
{
    public class AuthorizationModel
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Token { get; set; }
    }
}
