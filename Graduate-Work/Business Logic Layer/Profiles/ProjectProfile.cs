using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Profiles
{
    public class ProjectProfile : Profile
    {
        public ProjectProfile()
        {
            CreateMap<Project, ProjectDTO>().ForMember(pd => pd.Team, cfg => cfg.MapFrom(p => p.TeamMembers));
            CreateMap<ProjectDTO, Project>().ForMember(p => p.TeamMembers, cfg => cfg.MapFrom(pd => pd.Team));
        }
    }
}
