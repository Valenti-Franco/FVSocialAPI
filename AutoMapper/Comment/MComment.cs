using AutoMapper;
using SocialAPI.DTO.Comment;
using SocialAPI.Entities;

namespace SocialAPI.AutoMapper.Comment
{
    public class MComment : Profile
    {
        public MComment()
        {
       
            CreateMap<Entities.EComment, DTO.Comment.CommentDTO>();
            CreateMap<DTO.Comment.CommentDTO, Entities.EComment>();

            CreateMap<DTO.Comment.CommentCreateDTO, Entities.EComment>();
            CreateMap<Entities.EComment, DTO.Comment.CommentCreateDTO>();


        }
    }
}
