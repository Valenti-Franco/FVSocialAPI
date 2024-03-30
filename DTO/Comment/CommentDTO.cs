using SocialAPI.DTO.User;

namespace SocialAPI.DTO.Comment
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }

        public UserDTO User { get; set; }
    }
}
