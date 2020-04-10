using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using Business_Logic_Layer.Helpers;

namespace Business_Logic_Layer.Profiles
{
    public class TeamMemberProfile : Profile
    {
        public TeamMemberProfile()
        {
            CreateMap<TeamMember, TeamMemberDTO>().ForMember(t => t.Project, cfg => cfg.Ignore())
                .ForMember(t => t.Role, cfg => cfg.MapFrom(tm => tm.RoleId.Value.GetMemberByValue<RoleEnum>()));
            CreateMap<TeamMemberDTO, TeamMember>().ForMember(tm => tm.RoleId, cfg => cfg.MapFrom(t => (int)t.Role))
                .ForMember(t => t.Role, cfg => cfg.Ignore());
        }
    }
}
