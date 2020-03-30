using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using System;
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
                .ForMember(ed => ed.Patronymic, cfg => cfg.MapFrom(e => e.FullName.Split(' ', StringSplitOptions.None)[2]));
            CreateMap<EmployeeDTO, Employee>()
                .ForMember(e => e.FullName, cfg => cfg.MapFrom(ed => string.Join(' ', ed.FirstName, ed.SecondName, ed.Patronymic)));
        }
    }
}
