using AutoMapper;
using TreeStore.Models.Entities;
using TreeStore.Models.LoginModels;

namespace TreeStore.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Vlogin, LoginResponse>();
        }
    }
}
