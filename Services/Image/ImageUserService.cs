using AutoMapper;
using SocialAPI.DBContexts;
using SocialAPI.Entities;
using SocialAPI.Mapper.User;

namespace SocialAPI.Services.Image
{
    public class ImageUserService : ImageUserInterface
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public ImageUserService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void AddImageUser(EImageUser imageUser)
        {
            _context.ImageUser.Add(imageUser);
        }
        public void AddImageHeader(EImageHeader imageUser)
        {
            _context.ImageHeader.Add(imageUser);
        }


        public void DeleteImageUser(int id)
        {
            _context.ImageUser.Remove(GetImageUser(id));
        }
        public void DeleteImageHeader(int id)
        {
            _context.ImageHeader.Remove(GetImageHeader(id));
        }

        public EImageUser GetImageUser(int idUser)
        {
            //obtiene la imagen del usuario
            return _context.ImageUser.Where(p => p.UserId == idUser).FirstOrDefault();
        }
        public EImageHeader GetImageHeader(int idUser)
        {
            return _context.ImageHeader.Where(p => p.UserId == idUser).FirstOrDefault();
        }

        public void UpdateImageUser(EImageUser imageUser)
        {
            _context.ImageUser.Update(imageUser);
        }

        public void UpdateImageHeader(EImageHeader imageUser)
        {
            _context.ImageHeader.Update(imageUser);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

       
    }
}
