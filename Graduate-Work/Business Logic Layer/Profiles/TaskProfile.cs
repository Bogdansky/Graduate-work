using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using Business_Logic_Layer.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Business_Logic_Layer.Enums;

namespace Business_Logic_Layer.Profiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, TaskDTO>()
                .ForMember(m => m.TaskType, cfg => cfg.MapFrom(o => o.TaskTypeId.GetMemberByValue<TaskTypeEnum>()));
            CreateMap<TaskDTO, Task>()
                .ForMember(m => m.TaskType, cfg => cfg.MapFrom(o => new TaskType { Id = (int)o.TaskType, Name = o.TaskType.GetDescription()}));
        }
    }
}
