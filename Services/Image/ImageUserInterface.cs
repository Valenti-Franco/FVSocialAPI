using SocialAPI.Entities;

namespace SocialAPI.Services.Image
{

    public interface ImageUserInterface
    {
        public EImageUser GetImageUser(int id);
        public EImageHeader GetImageHeader(int userid);

        
        public void AddImageUser(EImageUser imageUser);

        public void AddImageHeader(EImageHeader imageUser);

        
        public void UpdateImageUser(EImageUser imageUser);

        public void UpdateImageHeader(EImageHeader imageUser);

        

        public void DeleteImageUser(int id);
        public void DeleteImageHeader(int id);
        public void SaveChanges();


    }
}
