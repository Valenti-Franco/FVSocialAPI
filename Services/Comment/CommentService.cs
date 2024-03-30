using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialAPI.DBContexts;
using SocialAPI.DTO.Comment;
using SocialAPI.Entities;

namespace SocialAPI.Services.Comment
{
    public class CommentService : CommentInterface
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public CommentService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void CreateComment(EComment comment)
        {
            try
            {
                _context.Comment.Add(comment);
            }
           // tirar error 404
            catch (DbUpdateException e)
            {
                throw new DbUpdateException("Error al crear el comentario", e);
            }
          
        }

   
        

        public async Task<List<CommentDTO>> GetComments(int postId)
        {
            var comments = await _context.Comment.Where(c => c.PostId == postId).ToListAsync();
            return _mapper.Map<List<CommentDTO>>(comments);
        }

        public void SaveChanges()
        {

            _context.SaveChanges();
        }

        public EComment GetComment(int id)
        {
           return _context.Comment.FirstOrDefault(c => c.Id == id);
        }

        public void DeleteComment(EComment comment)
        {
            _context.Comment.Remove(comment);
        }
    }
}
