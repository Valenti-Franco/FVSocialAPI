using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialAPI.DTO.Image;
using SocialAPI.DTO.User;
using SocialAPI.Entities;
using SocialAPI.Services.Image;
using SocialAPI.Services.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SocialAPI.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config; //Agregamos el IConfiguration para poder acceder a la configuración del appsettings.json
        private readonly UserInterface _repository;
        private readonly ImageUserInterface _ImageService;
        private readonly IMapper _mapper;


        public UserController(IConfiguration config, UserInterface repository, IMapper mapper, ImageUserInterface ImageService) //Agregamos el IConfiguration al constructor
        {
            _config = config;
            _repository = repository;
            _mapper = mapper;
            _ImageService = ImageService;
        }

        [HttpPost("RefreshToken")]
        public async Task<ActionResult<string>> efreshToken([FromBody] RefreshTokenDTO model)
        {
            var refreshToken = model.RefreshToken;
            if (refreshToken == null)
            {
                return BadRequest("No refreshToken");
            }
            var user = await _repository.RefreshToken(refreshToken);
            if (user == null)
            {
                return BadRequest("Error refreshToken");
            }
            var token = GenerateToken(user);

            return Ok(new
            {
                token = token,

            });

        }
        public class GoogleAuthRequest
        {
            public string TokenId { get; set; }
        }
        [HttpPost("googleAuth")]
        public async Task<ActionResult<string>> AutenticarGoogle([FromBody] GoogleAuthRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.TokenId,
                    new GoogleJsonWebSignature.ValidationSettings()
                    {
                        Audience = new List<string>() { "{your client google}" }
                    });

                var userId = payload.Subject;
                var email = payload.Email;
                var username = payload.Name;
                var Image = payload.Picture;

                var user = _repository.GetUserByEmail(email);

                if (user == null)
                {
                    user = new EUser(username)
                    {
                        Name = username,
                        Email = email,
                        Role = "User",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        verifyToken = CreateRandomToken(),
                        Status = true,
                    };
                    _repository.AddUser(user);
                    _repository.SaveChange();

                    EImageUser imagenNueva = new EImageUser
                    {
                        UserId = user.Id,
                        Image = Image,
                        publicId = "google",
                    };
                    _ImageService.AddImageUser(imagenNueva);
                    _ImageService.SaveChanges();
                }

                var token = GenerateToken(user);
                var refreshToken = GenerateRefreshToken();
                SetRefreshToken(refreshToken, user);
         

                return Ok(new
                {
                    status = 200,
                    token = token,
                    refreshToken = refreshToken.Token
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized(ex.InnerException.Message);
            }
        }
        [HttpPost("authenticate")]
        public ActionResult<string> Autenticar(UserLoginDTO userLogin) //Enviamos como parámetro la clase que creamos arriba
        {
            ////Paso 1: Validamos las credenciales
            var user = ValidateCredentials(userLogin);

            if (user is null)
                return Unauthorized("User or Password incorrect");

            if (!user.Status)
            {
                return BadRequest("No Verificado!");
            }

            var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"])); //Traemos la SecretKey del Json. agregar antes: using Microsoft.IdentityModel.Tokens;

            var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

            //Los claims son datos en clave->valor que nos permite guardar data del usuario.
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.Id.ToString())); //"sub" es una key estándar que significa unique user identifier, es decir, si mandamos el id del usuario por convención lo hacemos con la key "sub".
            claimsForToken.Add(new Claim("given_name", user.Username)); //Lo mismo para given_name y family_name, son las convenciones para nombre y apellido. Ustedes pueden usar lo que quieran, pero si alguien que no conoce la app
            claimsForToken.Add(new Claim("Email", user.Email)); //quiere usar la API por lo general lo que espera es que se estén usando estas keys.
            claimsForToken.Add(new Claim("role", user.Role ?? "Admin")); //Debería venir del usuario

            var jwtSecurityToken = new JwtSecurityToken( //agregar using System.IdentityModel.Tokens.Jwt; Acá es donde se crea el token con toda la data que le pasamos antes.
              _config["Authentication:Issuer"],
              _config["Authentication:Audience"],
              claimsForToken,
              DateTime.UtcNow,
              DateTime.UtcNow.AddHours(3),
              credentials);

            var tokenToReturn = new JwtSecurityTokenHandler() //Pasamos el token a string
                .WriteToken(jwtSecurityToken);

            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken, user);

            //retornamos el token y el refreshToken
            // dar el status y el token y el refreshToken
            return Ok(new
            {
                status = 200,
                token = tokenToReturn,
                refreshToken = refreshToken.Token
            });
        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow
                };
            }
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, EUser user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires,
            };

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);


            user.refreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
            _repository.SaveChange();
        }
        //[HttpGet("{page}")]
        //public async Task<IActionResult> WhoFollow(int page)
        //{
        //    var users = await _repository.GetUsersFollow(page);
        //    return Ok(users);
        //}
        [Authorize]
        [HttpGet]
        public ActionResult<UserDTO> GetUser()
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _repository.GetUser(int.Parse(userid));
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }


        [HttpGet("Admin/{page}/{name}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdmin(int page, string? name)
        {
            return Ok(await _repository.GetUsersAdmin(page, name));
        }

        [HttpGet("{page}/{name}")]
        public async Task<IActionResult> Get(int page, string? name)
        {
            return Ok(await _repository.GetUsers(page, name));
        }

        private EUser? ValidateCredentials(UserLoginDTO authParams)
        {
            return _repository.ValidateUser(authParams);
        }


        [HttpPost("Register")]
        //[Authorize]
        public ActionResult<UserDTO> CreacionUsuario(UserCreateDTO userCreate)
        {

            if (!_repository.ValidateEmail(userCreate.Email))
            {
                return BadRequest("Email is Exist");
            }

            if (!_repository.ValidateUsername(userCreate.Username))
            {
                return BadRequest("Username is Exist");
            }

            if(_repository.ValidatePassword(userCreate.Password))
            {
                return BadRequest("Password is not valid");
            }


            string passHash = BCrypt.Net.BCrypt.HashPassword(userCreate.Password);
            EUser UserNew = _mapper.Map<EUser>(userCreate);

            UserNew.Password = passHash;

            UserNew.verifyToken = CreateRandomToken();

            UserNew.CreatedAt = DateTime.Now;
    


            _repository.AddUser(UserNew);
            _repository.SaveChange();

            bool enviado = _repository.SendVerify(UserNew.Email, UserNew.verifyToken);

            if (enviado)
            {
                return Ok("An email message with the verification token has been sent");
            }
            else
            {
                return BadRequest("The email could not be sent.");
            }
            
        }
        [HttpPost("VerifyEmail/{token}")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var user = _repository.VerifyEmail(token);
            if (user == null)
            {
                return BadRequest("Token Invalido");

            }
            user.Status = true;
            _repository.SaveChange();

            return Ok("Usuario Verificado");

        }
        // by id get uer
        [HttpGet("{id}")]
        public ActionResult<UserDTO> GetUser(int id)
        {
            var user = _repository.GetUser(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<UserDTO>(user));
        }




        [HttpPut("{id}")]
        [Authorize]
        public ActionResult UpdateUser(int id, UserUpdateDTO userCreate)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExist = _repository.GetUser(id);
            if (userid == null)
            {
                return Unauthorized();
            }
            if (userExist == null)
            {
                return NotFound();
            }
            if (userExist.Id != int.Parse(userid))
            {
                return Unauthorized();
            }

            //actuñizar el post
            userExist.Name = userCreate.Name;
            userExist.LastName = userCreate.LastName;
            userExist.UpdatedAt = System.DateTime.Now;
            _repository.SaveChange();
            var userDTO = _mapper.Map<UserDTO>(userExist);
            return Ok(userDTO);
        }
        [HttpGet("Follow/{id}")]
        [Authorize]
        public ActionResult GetFollow(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExist = _repository.GetUser(id);
            if (userid == null)
            {
                return Unauthorized();
            }
            if (userExist == null)
            {
                return NotFound();
            }
            if (userExist.Id == int.Parse(userid))
            {
                return Unauthorized();
            }
            var user = _repository.GetUser(int.Parse(userid));
            if (user == null)
            {
                return NotFound();
            }
            var follow = _repository.GetFollow(int.Parse(userid), id);
            if (follow == null)
            {
                return Ok(false);
            }
            return Ok(true);
        }
        [HttpGet("GetWhoToFollow/{pages}")]
        [Authorize]
        public async Task<IActionResult> GetWhoToFollow(int pages)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid == null)
            {
                return Unauthorized();
            }
            var user = _repository.GetUser(int.Parse(userid));
            if (user == null)
            {
                return NotFound();
            }
            var users =  _repository.GetWhoToFollow(int.Parse(userid), pages);
            return Ok(users);
        }
       

        [HttpPost("Follow/{id}")]
        [Authorize]
        public ActionResult FollowUser(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExist = _repository.GetUser(id);
            if (userid == null)
            {
                return Unauthorized();
            }
            if (userExist == null)
            {
                return NotFound();
            }
            if (userExist.Id == int.Parse(userid))
            {
                return BadRequest("You can't follow yourself");
            }
            var user = _repository.GetUser(int.Parse(userid));
            if (user == null)
            {
                return NotFound();
            }
            var follow = _repository.GetFollow(int.Parse(userid), id);
            if (follow != null)
            {
                return BadRequest("Ya sigues a este usuario");
            }
            var followNew = new EFollowing
            {
                FollowerId = int.Parse(userid),
                FollowedId = id,

            };
            _repository.Follow(followNew);

            user.Following++;
            userExist.Followers++;
            _repository.SaveChange();
            return NoContent();
        }
        [HttpDelete("UnFollow/{id}")]
        [Authorize]
        public ActionResult UnFollowUser(int id)
        {
            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userExist = _repository.GetUser(id);
            if (userid == null)
            {
                return Unauthorized();
            }
            if (userExist == null)
            {
                return NotFound();
            }
            if (userExist.Id == int.Parse(userid))
            {
                return BadRequest("You can't unfollow yourself");
            }
            var user = _repository.GetUser(int.Parse(userid));
            if (user == null)
            {
                return NotFound();
            }
            var follow = _repository.GetFollow(int.Parse(userid), id);
            if (follow == null)
            {
                return BadRequest("You do not follow this user");
            }
            _repository.UnFollow(follow);
            user.Following--;
            userExist.Followers--;
            _repository.SaveChange();
            return NoContent();
        }

        [HttpPost("Image/Header")]
        [Authorize]
        public async Task<IActionResult> CreateImageHeader(ImageCreateDTO imageCreate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ValidateSizeBase64(imageCreate.Base64))
            {
                return BadRequest("Base64 file size exceeds 1 MB limit.");
            }

            // Verificar si ya existe una imagen para el usuario con el mismo id
            // userid pasar a Int
            var imageExist = _ImageService.GetImageHeader(int.Parse(userId));

            if (imageExist != null)
            {
                // Actualizar los datos de la imagen existente
                var result = await Upload(imageCreate.Base64);
                imageExist.publicId = result.publicId;
                imageExist.Image = result.url;
                _ImageService.UpdateImageHeader(imageExist);
                _ImageService.SaveChanges();

                var userImage = _mapper.Map<ImageCreateDTO>(imageExist);
                return Ok(userImage);
            }
            else
            {
                // Si no existe, crear una nueva imagen
                EImageHeader imagenNueva = _mapper.Map<EImageHeader>(imageCreate);
                imagenNueva.UserId = int.Parse(userId);
                var result = await Upload(imageCreate.Base64);
                imagenNueva.publicId = result.publicId;
                imagenNueva.Image = result.url;
                _ImageService.AddImageHeader(imagenNueva);
                _ImageService.SaveChanges();

                var userImage = _mapper.Map<ImageUserDTO>(imagenNueva);
                return Ok(userImage);
            }
        }

        [HttpPost("Image/Profile")]
        [Authorize]
        public async Task<IActionResult> CreateImageUserProfile(ImageCreateDTO imageCreate)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (!ValidateSizeBase64(imageCreate.Base64))
            {
                return BadRequest("Base64 file size exceeds 1 MB limit.");
            }

            // Verificar si ya existe una imagen para el usuario con el mismo id
            // userid pasar a Int
            var imageExist = _ImageService.GetImageUser(int.Parse(userId));

            if (imageExist != null)
            {


                // Actualizar los datos de la imagen existente
                var result = await Upload(imageCreate.Base64);
                imageExist.publicId = result.publicId;
                imageExist.Image = result.url;
                _ImageService.UpdateImageUser(imageExist);
                _ImageService.SaveChanges();

                var userImage = _mapper.Map<ImageUserDTO>(imageExist);
                return Ok(userImage);
            }
            else
            {

                // Si no existe, crear una nueva 

            

                EImageUser imagenNueva = _mapper.Map<EImageUser>(imageCreate);
                imagenNueva.UserId = int.Parse(userId);
                var result = await Upload(imageCreate.Base64);
                imagenNueva.publicId = result.publicId;
                imagenNueva.Image = result.url;
                _ImageService.AddImageUser(imagenNueva);
                _ImageService.SaveChanges();


                var userImage = _mapper.Map<ImageUserDTO>(imageExist);
                return Ok(userImage);
            }
        }

        private async Task<ActionResult> DeletePhoto( string publicId)
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
            return ( url, publicId);

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

        private string GenerateToken(EUser user)
        {
            var securityPassword = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["Authentication:SecretForKey"])); 

            var credentials = new SigningCredentials(securityPassword, SecurityAlgorithms.HmacSha256);

          
            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.Id.ToString())); 
            claimsForToken.Add(new Claim("given_name", user.Username)); 
            claimsForToken.Add(new Claim("Email", user.Email)); 
            claimsForToken.Add(new Claim("role", user.Role ?? "Admin")); 

            var jwtSecurityToken = new JwtSecurityToken(
                _config["Authentication:Issuer"],
                _config["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(3),
                 credentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtSecurityToken);
            return tokenToReturn.ToString();

        }
        private string CreateRandomToken()
        {
            byte[] tokenBytes = new byte[32]; // 16 bytes para un token de 32 caracteres
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            return BitConverter.ToString(tokenBytes).Replace("-", "").ToLower();
        }
    }
}
