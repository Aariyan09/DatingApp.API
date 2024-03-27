using AutoMapper;
using DatingApp.API.DTOs.Requests;
using DatingApp.API.DTOs.Response;
using DatingApp.API.Entities;
using DatingApp.API.Extenstions;

namespace DatingApp.API.Helpers
{
    public class AutoMapper_Profiles : Profile
    {
        public AutoMapper_Profiles() 
        {
            CreateMap<AppUser, Member_DTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(dest => dest.Age,opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                ;
            CreateMap<Photo, Photo_DTO>();
            CreateMap<Photo_DTO, Photo>();
            CreateMap<MemberUpdate_DTO, AppUser>();
        }
    }
}
