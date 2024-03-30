using AutoMapper;

namespace SocialAPI.AutoMapper.Like
{
    public class MLike : Profile
    {
        public MLike()
        {
            CreateMap<Entities.ELike, DTO.Like.LikeDTO>();
            CreateMap<DTO.Like.LikeDTO, Entities.ELike>();
        }
    }
}
