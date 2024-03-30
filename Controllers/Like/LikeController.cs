using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialAPI.DTO.Like;
using SocialAPI.Services.Like;
using System.Security.Claims;

namespace SocialAPI.Controllers.Like
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly LikeInterface _likeService;
        private readonly IMapper _mapper;

        public LikeController(LikeInterface likeService, IMapper mapper)
        {
            _likeService = likeService;
            _mapper = mapper;
        }
      

        [HttpGet("{postId}")]
        [Authorize]
        public ActionResult GetLikes(int postId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid == null)
            {
                return BadRequest();
            }
            var likes = _likeService.GetLikes(postId);
            var response = _mapper.Map<List<LikeDTO>>(likes);
            return Ok(response);
        }
        [HttpGet("/Get/{postId}")]
        [Authorize]
        public ActionResult GetLike(int postId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid == null)
            {
                return BadRequest();
            }
            var likes = _likeService.GetLike(int.Parse(userid), postId);
            var response = _mapper.Map<LikeDTO>(likes);
            return Ok(response);
        }

        [HttpPost("{postId}")]
        [Authorize]
        public ActionResult CreateLike(int postId)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid == null)
            {
                return BadRequest();
            }
            var existLike = _likeService.GetLike(int.Parse(userid), postId);
            if (existLike == null)
            {
                _likeService.addLikePost(int.Parse(userid), postId);
                _likeService.SaveChanges();
                return new OkObjectResult(new { message = true });
            }
            else
            {
                _likeService.deleteLikePost(int.Parse(userid), postId);
                _likeService.SaveChanges();
                return new OkObjectResult(new { message = false });
            }
        }

        
    }
}
