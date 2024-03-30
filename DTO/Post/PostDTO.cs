using SocialAPI.DTO.Comment;
using SocialAPI.DTO.Image;
using SocialAPI.DTO.User;

namespace SocialAPI.DTO.Post
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public UserDTO User { get; set; }

        public ICollection<ImagePostDTO> ImagePosts { get; set; }

        public ICollection<CommentDTO> Comments { get; set; }

        public int LikesCount { get; set; }

        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

    }
}
