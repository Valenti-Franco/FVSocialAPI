using SocialAPI.Entities;

namespace SocialAPI.Services.Like
{
    public interface LikeInterface
    {
        public ELike GetLike(int idUser,int id);

        public List<ELike> GetLikes(int idPost);
        public void addLikePost(int idUser,int idPost);

        public void deleteLikePost(int userid, int id);

        public void SaveChanges();


    }
}
