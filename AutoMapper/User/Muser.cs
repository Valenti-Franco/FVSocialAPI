using AutoMapper;

namespace SocialAPI.Mapper.User
{
    public class Muser : Profile
    {
        public Muser()
        {
            CreateMap<Entities.EUser, DTO.User.UserDTO>();
            CreateMap<DTO.User.UserDTO, Entities.EUser>();

            CreateMap<Entities.EUser, DTO.User.UsersDTO>();
            CreateMap<DTO.User.UsersDTO, Entities.EUser>();

            CreateMap<DTO.User.UserLoginDTO, Entities.EUser>();

            CreateMap<DTO.User.UserCreateDTO, Entities.EUser>();
            CreateMap<Entities.EUser, DTO.User.UserCreateDTO>();

           

        }
    }
}
