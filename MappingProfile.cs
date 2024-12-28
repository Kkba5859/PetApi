using AutoMapper;
using PetApi.Models;

namespace PetApi
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Маппинг между User и UserDto
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
