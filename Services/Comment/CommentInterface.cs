using SocialAPI.DTO.Comment;
using SocialAPI.Entities;

namespace SocialAPI.Services.Comment
{
    public interface CommentInterface
    {
        void CreateComment( EComment comment);
        void DeleteComment(EComment comment);
        public EComment GetComment(int id);
        public Task<List<CommentDTO>> GetComments(int postId);
        void SaveChanges();
    }
}
