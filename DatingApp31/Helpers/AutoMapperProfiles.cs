using AutoMapper;
using DatingApp31.Dtos;
using DatingApp31.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp31.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>().ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                                             .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailedDto>().ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                                                 .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
        }
    }
}
