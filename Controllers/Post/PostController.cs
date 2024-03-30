using AutoMapper;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialAPI.DTO.Image;
using SocialAPI.DTO.Post;
using SocialAPI.Entities;
using SocialAPI.Services.Post;
using System.Security.Claims;
using SocialAPI.Services.Image;

namespace SocialAPI.Controllers.Post
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostInterface _postService;
        private readonly ImagePostInterface _ImageService;
        private readonly IMapper _mapper;

        public PostController(PostInterface postService, IMapper mapper, ImagePostInterface ImageService)
        {
            _postService = postService;
            _mapper = mapper;
            _ImageService = ImageService;
        }


        //public async Task<IActionResult> Get(int page, string? name)
        //{
        //    return Ok(await _repository.GetUsers(page, name));
        //}
        // get para solo followings
        [HttpGet("Follow/{page}")]
        public async Task<ActionResult> GetPostFollow(int page)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok(await _postService.GetPostFollow(int.Parse(userid), page));
        }



        [HttpGet("Profile/{idUser}/{page}")]
        [Authorize]
        public async Task<ActionResult> GetPostProfile(int idUser,int page)
        {
            
            return Ok(await _postService.GetPostProfile(idUser, page));
        }
        [HttpGet("Content/{page}")]
        [Authorize]
        public async Task<ActionResult> GetPostContent(int page)
        {
            return Ok(await _postService.GetPostContent(page));
        }

        [HttpGet("{id}")]
        [Authorize]

        public ActionResult GetPost(int id)
        {
            var post = _mapper.Map<PostDTO>(_postService.GetPost(id));
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
        [HttpGet("{page}/{content}")]
        [Authorize]

        public async Task<ActionResult> GetPost(int page, string? content)
        {
            return Ok(await _postService.GetPost(page, content));
        }
        [HttpDelete("{id}")]
        [Authorize]
        public ActionResult DeletePost(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var postExist = _postService.GetPost(id);
            if (userid == null)
            {
                return Unauthorized();
            }
            if (postExist == null)
            {
                return NotFound();
            }
            if (postExist.UserId != int.Parse(userid))
            {
                return Unauthorized();
            }
            _postService.DeletePost(id);
            _postService.SaveChange();
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize]
        public ActionResult UpdatePost(int id, PostCreateDTO postCreate)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var postExist = _postService.GetPost(id);
            if (userid == null)
            {
                return Unauthorized();
            }
            if (postExist == null)
            {
                return NotFound();
            }
            if (postExist.UserId != int.Parse(userid))
            {
                return Unauthorized();
            }
        
            //actuñizar el post
            postExist.Content = postCreate.Content;
            postExist.UpdatedAt = System.DateTime.Now;
            _postService.SaveChange();
            var postDTO = _mapper.Map<PostDTO>(postExist);
            return Ok(postDTO);
        }
       

        [HttpPost("Create")]
        [Authorize]
        public async Task<ActionResult> CreatePost(PostCreateDTO postCreate)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var post = _mapper.Map<EPost>(postCreate);
            post.CreatedAt = System.DateTime.Now;
            post.UserId = int.Parse(userid);


            var newPost = _postService.CreatePost(post);
            _postService.SaveChange();
            return Ok(newPost);
        }

        [HttpPost("Image/{idPost}")]
        [Authorize]
        public async Task<IActionResult> CreateImageUserProfile(ImageCreateDTO imageCreate, int idPost)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var postExist = _postService.GetPost(idPost);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (postExist == null)
            {
                return NotFound();
            }

            if (postExist.UserId != int.Parse(userId))
            {
                return Unauthorized();
            }


            if (!ValidateSizeBase64(imageCreate.Base64))
            {
                return BadRequest("Base64 file size exceeds 1 MB limit.");
            }



            if (imageCreate == null)
            {
                return BadRequest("imageCreate is null.");
            }
            int postsCountWithImages;
            _postService.LimitImg(int.Parse(userId), out postsCountWithImages);

            if (postsCountWithImages >= 3)
            {
                return BadRequest($"The limit of posts with images in the last 7 days has been exceeded. Number of posts with images: {postsCountWithImages}");
            }

            EImagePost imagenNueva = _mapper.Map<EImagePost>(imageCreate);

            if (imagenNueva == null)
            {
                return BadRequest("imagenNueva is null.");
            }
            imagenNueva.PostId = idPost;
            imagenNueva.Post = postExist;
            var result = await Upload(imageCreate.Base64);
                imagenNueva.publicId = result.publicId;
                imagenNueva.Image = result.url;
                _ImageService.AddImagePost(imagenNueva);
                _ImageService.SaveChanges();


                var postImage = _mapper.Map<ImagePostDTO>(imagenNueva);
                return Ok(postImage);
        

        }
        private async Task<ActionResult> DeletePhoto(string publicId)
        {
            Account account = new Account
              (
            //add your Cloudinary
            "{your-User}",
            "{your-id}",
            "{your-token}"
             );

            Cloudinary cloudinary = new Cloudinary(account);

            var deleteParams = new DeletionParams(publicId);
            var result = await cloudinary.DestroyAsync(deleteParams);
            if (result != null)
            {
                return Ok();
            }
            return BadRequest();
        }

        private async Task<(string url, string publicId)> Upload(string base64)
        {
            Account account = new Account(
            //add your Cloudinary
            "{your-User}",
            "{your-id}",
            "{your-token}");

            Cloudinary cloudinary = new Cloudinary(account);
            cloudinary.Api.Secure = true;
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(Guid.NewGuid().ToString(), new MemoryStream(Convert.FromBase64String(base64)))
            };
            var response = await cloudinary.UploadAsync(uploadParams);
            var url = response.SecureUrl.AbsoluteUri;
            var publicId = response.PublicId;
            return (url, publicId);

        }
        private bool ValidateSizeBase64(string base64String)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64String);
                int maxSizeInBytes = 1024 * 1024; // 2 MB
                return bytes.Length <= maxSizeInBytes;
            }
            catch (FormatException)
            {

                return false;
            }
        }
    }
}
