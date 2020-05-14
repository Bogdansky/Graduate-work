using AutoMapper;
using Business_Logic_Layer.DTO;
using Data_Access_Layer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic_Layer.Profiles
{
    public class TrackingHistoryProfile : Profile
    {
        public TrackingHistoryProfile()
        {
            CreateMap<TrackingHistory, TrackingHistoryDTO>();
            CreateMap<TrackingHistoryDTO, TrackingHistory>();
        }
    }
}
