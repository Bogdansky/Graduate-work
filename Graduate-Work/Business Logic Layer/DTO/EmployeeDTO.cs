using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class EmployeeDTO : BaseModelDTO
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Patronymic { get; set; }
        public DateTime Birthday { get; set; }
        public int? UserId { get; set; }
        public int RoleId { get; set; }
        public UserDTO User { get; set; }
        public RoleEnum Role { get; set; }
        public ICollection<TeamMemberDTO> Projects { get; set; }
    }
}
