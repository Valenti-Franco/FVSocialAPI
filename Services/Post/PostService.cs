using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialAPI.DBContexts;
using SocialAPI.DTO.Post;
using SocialAPI.DTO.User;
using SocialAPI.Entities;
using System.Globalization;

namespace SocialAPI.Services.Post
{
    public class PostService : PostInterface
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public PostService(Context context, IMapper mapper)
        {
            _context = context;
            _context = context;
            _mapper = mapper;
        }
        
        public PostDTO CreatePost(EPost post)
        {
            _context.Post.Add(post);
            _context.SaveChanges();
            var id = post.Id;
            var postCreate = GetPost(id);

            var response = _mapper.Map<PostDTO>(postCreate);

            return response;

        }

        public void DeletePost(int id)
        {
            _context.Post.Remove(GetPost(id));
        }

        public async Task<ActionResult<PostResponseDTO>> GetPost(int page, string? content)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.Post.Count() / pageResult);
            var posts = await _context.Post
                .Where(p => content == null || p.Content.Contains(content))
                .Include(p => p.ImagePosts)
                .Include(p => p.Comments)
                .Include(p => p.User)
                .ThenInclude(p => p.ImageUsers)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .ToListAsync();


            var response = new PostResponseDTO
            {
                Posts = _mapper.Map<List<PostDTO>>(posts),
                CurrentPage = page,
                Pages = (int)pageCount

            };

            return response;
        }
        public async Task<ActionResult<PostResponseDTO>> GetPostFollow(int idUser, int page)
        {
            var pageResult = 8f;
            var pageCount = Math.Ceiling(_context.Post.Count() / pageResult);

            var followingUserIds = _context.Following
                .Where(f => f.FollowerId == idUser)
                .Select(f => f.FollowedId)
                .ToList();

            // Agrega el ID del usuario logeado a la lista
            followingUserIds.Add(idUser);

            var posts = await _context.Post
                .Where(p => followingUserIds.Contains(p.UserId))
                .Include(p => p.ImagePosts)
                .Include(p => p.Comments)
                .Include(p => p.User)
                    .ThenInclude(p => p.ImageUsers)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .ToListAsync();

            var response = new PostResponseDTO
            {
                Posts = _mapper.Map<List<PostDTO>>(posts),
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return response;
        }

        public EPost GetPost(int id)
        {
            var post = _context.Post
                .Include(p => p.ImagePosts)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Include(p => p.User)
                    .ThenInclude(u => u.ImageUsers)
                .FirstOrDefault(p => p.Id == id);

            var commentUserIds = post.Comments.Select(c => c.User.Id).ToList();

            var usersWithImages = _context.User
                .Where(u => commentUserIds.Contains(u.Id))
                .Include(u => u.ImageUsers)
                .ToList();

            foreach (var comment in post.Comments)
            {
                comment.User = usersWithImages.First(u => u.Id == comment.User.Id);
            }

            return post;
        }

        public async Task<ActionResult<PostResponseDTO>> GetPostContent(int page)
        {
            var pageResult = 8f;
            var pageCount = Math.Ceiling(_context.Post.Count() / pageResult);
            var posts = await _context.Post
                .Include(p => p.ImagePosts)
                .Include(p => p.Comments)
                .Include(p => p.User)
                .ThenInclude(p => p.ImageUsers)
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .ToListAsync();
            var response = new PostResponseDTO
            {
                Posts = _mapper.Map<List<PostDTO>>(posts),
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return response;
        }
        public async Task<ActionResult<PostResponseDTO>> GetPostProfile(int id, int page)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.Post.Where(p => p.UserId == id).Count() / pageResult);
            var posts = await _context.Post
                .Where(p => p.UserId == id)
                .Include(p => p.ImagePosts)
                .Include(p => p.Comments)
                .ThenInclude(c => c.User)
                .Include(p => p.User)
                .ThenInclude(p => p.ImageUsers)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            var response = new PostResponseDTO
            {
                Posts = _mapper.Map<List<PostDTO>>(posts),
                CurrentPage = page,
                Pages = (int)pageCount
            };

            return response;
        }
        public void AddLike(int idPost)
        {
            var post = GetPost(idPost);
            post.LikesCount++;
            
        }
        public void DeleteLike(int idPost)
        {
            var post = GetPost(idPost);
            post.LikesCount--;
        }
        public void SaveChange()
        {
            _context.SaveChanges();
        }

        public void LimitImg(int idUser, out int postsCount)
        {
            DateTime currentDate = DateTime.Now;
            DateTime limitDate = currentDate.AddDays(-7);
            postsCount = 0;

            foreach (var p in _context.Post.Include(p => p.ImagePosts))
            {
                if (p.UserId == idUser && p.CreatedAt > limitDate)
                {
                    if (p.ImagePosts.Any())
                    {
                        postsCount++;
                    }
                }
            }
        }
    }
}
