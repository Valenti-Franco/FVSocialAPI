using AutoMapper;

namespace SocialAPI.AutoMapper.Posts
{
    public class MPost : Profile
    {
        public MPost()
        {
            CreateMap<Entities.EPost, DTO.Post.PostDTO>();
            CreateMap<DTO.Post.PostDTO, Entities.EPost>();

            CreateMap<DTO.Post.PostCreateDTO, Entities.EPost>();
            CreateMap<Entities.EPost, DTO.Post.PostCreateDTO>();

            CreateMap<DTO.Post.PostEditDTO, Entities.EPost>();
            CreateMap<Entities.EPost, DTO.Post.PostEditDTO>();


        }

    }
}
