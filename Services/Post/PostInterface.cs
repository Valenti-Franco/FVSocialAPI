using Microsoft.AspNetCore.Mvc;
using SocialAPI.DTO.Post;
using SocialAPI.Entities;

namespace SocialAPI.Services.Post
{
    public interface PostInterface
    {
        public Task<ActionResult<PostResponseDTO>> GetPost(int page, string? content);

        public void LimitImg(int idUser, out int postsCount);
        public EPost GetPost(int id);

        void DeletePost(int id);

        public Task<ActionResult<PostResponseDTO>> GetPostProfile(int id, int page);

        public Task<ActionResult<PostResponseDTO>> GetPostContent( int page);

        public Task<ActionResult<PostResponseDTO>> GetPostFollow(int idUser, int page);

        
        public PostDTO CreatePost(EPost post);

        void AddLike(int idPost);
        void DeleteLike(int idPost);
        void SaveChange();
    }
}
