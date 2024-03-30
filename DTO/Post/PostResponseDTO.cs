using SocialAPI.DTO.Comment;
using SocialAPI.DTO.Image;
using SocialAPI.DTO.User;

namespace SocialAPI.DTO.Post
{
    public class PostResponseDTO
    {
        public List<PostDTO> Posts { get; set; }

        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
