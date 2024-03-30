using AutoMapper;
using SocialAPI.DBContexts;
using SocialAPI.Entities;

namespace SocialAPI.Services.Image
{
    public class ImagePostService : ImagePostInterface
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public ImagePostService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public void AddImagePost(EImagePost imagePost)
        {
           
            _context.ImagePost.Add(imagePost);
        }

        public void DeleteImagePost(int id)
        {
            _context.ImagePost.Remove(GetImagePost(id));
        }

        public EImagePost GetImagePost(int id)
        {
            return _context.ImagePost.Find(id);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void UpdateImagePost(EImagePost imagePost)
        {
            _context.ImagePost.Update(imagePost);
        }
    }
}
