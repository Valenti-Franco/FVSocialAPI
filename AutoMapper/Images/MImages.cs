using AutoMapper;
using SocialAPI.DTO.Image;
using SocialAPI.Entities;

namespace SocialAPI.AutoMapper.Images
{
    public class MImages : Profile
    {
        public MImages()
        {
            CreateMap<Entities.EImageUser, DTO.Image.ImageUserDTO>();
            CreateMap<DTO.Image.ImageUserDTO, Entities.EImageUser>();

            CreateMap<Entities.EImageHeader, DTO.Image.ImageUserDTO>();
            CreateMap<DTO.Image.ImageUserDTO, Entities.EImageHeader>();

            CreateMap<DTO.Image.ImageCreateDTO, Entities.EImageUser>();
            CreateMap<Entities.EImageUser, DTO.Image.ImageCreateDTO>();

            CreateMap<DTO.Image.ImageCreateDTO, Entities.EImageHeader>();
            CreateMap<Entities.EImageHeader, DTO.Image.ImageCreateDTO>();


            CreateMap<Entities.EImagePost, DTO.Image.ImagePostDTO>();
            CreateMap<DTO.Image.ImagePostDTO, Entities.EImagePost>();

            CreateMap<DTO.Image.ImageCreateDTO, Entities.EImagePost>();
            CreateMap<Entities.EImagePost, DTO.Image.ImageCreateDTO>();






        }
    }
}
