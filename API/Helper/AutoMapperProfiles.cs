using API.DTO;
using API.Entities;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() {
            CreateMap<RegisterDTO, User>()
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.MapFrom((src, dst, arg3, context) => context.Items["PasswordHash"]))
                .ForMember(
                    dest => dest.PasswordSalt,
                    opt => opt.MapFrom((src, dst, arg3, context) => context.Items["PasswordSalt"]));

            CreateMap<User, AuthorizedUserDTO>()
                .ForMember(
                    dest => dest.Token,
                    opt => opt.MapFrom((src, dst, arg3, context) => context.Items["Token"]));

            CreateMap<User, UserDTO>();
        }
    }
}
