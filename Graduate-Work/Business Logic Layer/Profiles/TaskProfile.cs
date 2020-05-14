using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using Business_Logic_Layer.Helpers;
using Business_Logic_Layer.Enums;
using Business_Logic_Layer.Models;

namespace Business_Logic_Layer.Profiles
{
    public class TaskProfile : Profile
    {
        public TaskProfile()
        {
            CreateMap<Task, TaskDTO>()
                .ForMember(m => m.TaskType, cfg => cfg.MapFrom(o => o.TaskTypeId.GetMemberByValue<TaskTypeEnum>()))
                .ForMember(m => m.TaskStatus, cfg => cfg.MapFrom(o => o.TaskStatusId.GetMemberByValue<TaskStatusEnum>()));
            CreateMap<TaskDTO, Task>()
                .ForMember(m => m.TaskTypeId, cfg => cfg.MapFrom(o => (int)o.TaskType))
                .ForMember(m => m.TaskStatusId, cfg => cfg.MapFrom(o => (int)o.TaskStatus))
                .ForMember(m => m.TaskStatus, cfg => cfg.Ignore())
                .ForMember(m => m.TaskType, cfg => cfg.Ignore());
            CreateMap<Task, TrackedTask>().ForMember(tt => tt.PreviousRecent, cfg => cfg.MapFrom(t => t.Recent));
            CreateMap<TaskDTO, TrackedTask>().ForMember(tt => tt.PreviousRecent, cfg => cfg.MapFrom(td => td.Recent))
                                             .ForMember(tt => tt.NextRecent, cfg => cfg.MapFrom(td => td.Recent));
        }
    }
}
