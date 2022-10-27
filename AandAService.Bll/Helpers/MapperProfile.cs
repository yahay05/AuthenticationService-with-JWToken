using System;
using AandAService.Bll.Models.RequestModels;
using AandAService.Bll.Models.ResponseModels;
using AandAService.Dal.Entities;
using AutoMapper;

namespace AandAService.Bll.Helpers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<MyUser, RegistrationDto>().ReverseMap();
            CreateMap<MyUser, AuthenticationResponse>().ReverseMap();
            CreateMap<MyUser, UserViewModel>().ReverseMap();
        }
        
    }
}
