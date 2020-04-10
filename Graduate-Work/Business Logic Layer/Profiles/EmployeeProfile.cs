using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using System;
using Business_Logic_Layer.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Profiles
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDTO>()
                .ForMember(ed => ed.FirstName, cfg => cfg.MapFrom(e => e.FullName.Split(' ', StringSplitOptions.None)[0]))
                .ForMember(ed => ed.SecondName, cfg => cfg.MapFrom(e => e.FullName.Split(' ', StringSplitOptions.None)[1]))
                .ForMember(ed => ed.Patronymic, cfg => cfg.MapFrom(e => e.FullName.Split(' ', StringSplitOptions.None)[2]))
                .ForMember(ed => ed.Projects, cfg => cfg.MapFrom(e => e.TeamMembers))
                .ForMember(ed => ed.Role, cfg => cfg.MapFrom(e => e.RoleId.GetMemberByValue<RoleEnum>()));
            CreateMap<EmployeeDTO, Employee>()
                .ForMember(e => e.FullName, cfg => cfg.MapFrom(ed => string.Join(' ', ed.FirstName, ed.SecondName, ed.Patronymic)))
                .ForMember(e => e.Role, cfg => cfg.Ignore());
                //.ForMember(e => e.Role, cfg => cfg.MapFrom(ed => ed.RoleId == 0 ? new Role { Id = (int)ed.Role, Name = ed.Role.GetDescription()} : new Role { Id = ed.RoleId, Name = ed.RoleId.GetDescriptionByValue<RoleEnum>()}));
        }
    }
}
