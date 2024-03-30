using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialAPI.DTO.Comment;
using SocialAPI.Services.Comment;
using SocialAPI.Services.Post;
using System.Security.Claims;

namespace SocialAPI.Controllers.Comment
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly CommentInterface _commentService;
        private readonly PostInterface _postService;
        private readonly IMapper _mapper;

        public CommentController(CommentInterface commentService, IMapper mapper, PostInterface postService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _postService = postService;
        }

        [HttpGet("create/{postId}")]
        public ActionResult GetComments(int postId)
        {
            if (postId == 0)
            {
                return BadRequest();
            }

            if (_postService.GetPost(postId) == null)
            {
                return NotFound();
            }
            return Ok(_commentService.GetComments(postId));
        }

        [HttpPost]
        [Authorize]
        public ActionResult CreateComment(CommentCreateDTO comment)
        {
            var postId = comment.PostId;
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (postId == 0 || comment == null)
            {
                return BadRequest();
            }

            if (_postService.GetPost(postId) == null)
            {
                return NotFound();
            }
            if (comment.Content == null)
            {
                return BadRequest();
            }

            var commentCreate = _mapper.Map<Entities.EComment>(comment);

            commentCreate.UserId = int.Parse(userid);
            commentCreate.CreatedAt = System.DateTime.Now;

            _commentService.CreateComment(commentCreate);
            _commentService.SaveChanges();
            return Ok(_mapper.Map<CommentDTO>(commentCreate));
        }
        [HttpGet("{id}")]
        public ActionResult GetComment(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var comment = _commentService.GetComment(id);
            if (comment == null)
            {
                return NotFound();
            }
            var commentDTO = _mapper.Map<CommentDTO>(comment);
            return Ok(commentDTO);
        }

        [HttpPut("{id}/{comment}")]
        [Authorize]
        public ActionResult UpdateComment(int id, string comment)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == 0 || comment == null)
            {
                return BadRequest();
            }
            var commentUpdate = _commentService.GetComment(id);

            if (commentUpdate == null)
            {
                return NotFound();
            }
            if (commentUpdate.UserId != int.Parse(userid))
            {
                return Unauthorized();
            }
            if (commentUpdate.Content == comment)
            {
                return NoContent();
            }


            commentUpdate.Content = comment;
            commentUpdate.UpdatedAt = System.DateTime.Now;

            _commentService.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeleteComment(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == 0)
            {
                return BadRequest();
            }
            var comment = _commentService.GetComment(id);
            if (comment == null)
            {
                return NotFound();
            }
            if (comment.UserId != int.Parse(userid))
            {
                return Unauthorized();
            }
            _commentService.DeleteComment(comment);
            _commentService.SaveChanges();
            return Ok();
        }

    }
}
