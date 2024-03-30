using SocialAPI.Entities;

namespace SocialAPI.Services.Image
{
    public interface ImagePostInterface
    {
        public EImagePost GetImagePost(int id);
        public void AddImagePost(EImagePost imagePost);

        public void UpdateImagePost(EImagePost imagePost);

        public void DeleteImagePost(int id);

        public void SaveChanges();
    }
}
