using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.DTO
{
    public class EmployeeDTO
    {
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Patronymic { get; set; }
        public int? UserId { get; set; }
        public int? OrganizationId { get; set; }
        public UserDTO User { get; set; }
        public OrganizationDTO Organization { get; set; }
        public ICollection<TeamMemberDTO> Projects { get; set; }
    }
}
