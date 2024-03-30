using AutoMapper;
using SocialAPI.DBContexts;
using SocialAPI.Entities;
using SocialAPI.Services.Post;

namespace SocialAPI.Services.Like
{
    public class LikeService : LikeInterface
    {
        private readonly Context _context;
        private readonly PostInterface _postInterface;
        private readonly IMapper _mapper;

        public LikeService(Context context, IMapper mapper, PostInterface postInterface)
        {
            _context = context;
            _mapper = mapper;
            _postInterface = postInterface;
        }
        public ELike GetLike(int userid,int id)
        {
           return _context.Like.FirstOrDefault(l => l.UserId == userid && l.PostId == id);
        }

        public List<ELike> GetLikes(int idPost)
        {
            return _context.Like.Where(l => l.PostId == idPost).ToList();
        }

        public void addLikePost(int idUser,int idPost)
        {
            var like = new ELike
            {
                UserId = idUser,
                PostId = idPost
            };
            _postInterface.AddLike(idPost);
            _context.Add(like);
        }

        public void deleteLikePost(int userid, int id)
        {
            var like = GetLike(userid, id);
            _postInterface.DeleteLike(like.PostId);
            _context.Remove(like);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
