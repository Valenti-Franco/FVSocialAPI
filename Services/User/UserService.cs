using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MailKit.Net.Smtp;
using SocialAPI.DBContexts;
using SocialAPI.DTO.Image;
using SocialAPI.DTO.User;
using SocialAPI.Entities;
using SocialAPI.Services.Interface;
using System.Net.Mail;
using System.Text.RegularExpressions;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;


namespace SocialAPI.Services.User
{
    public class UserService : UserInterface
    {
        private readonly Context _context;
        private readonly IMapper _mapper;

        public UserService(Context context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;


        }

        public async Task<ActionResult<UserResponseDTO>> GetUsers(int page, string? name)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.User.Count() / pageResult);
            var users = await _context.User
                .Where(p => name == null || p.Username.Contains(name))
                .Include(p => p.ImageUsers)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .OrderBy(p => p.Followers)
                .ToListAsync();


            var response = new UserResponseDTO
            {
                Users = _mapper.Map<List<UsersDTO>>(users),
                CurrentPage = page,
                Pages = (int)pageCount

            };

            return response;


        }
        public async Task<ActionResult<UserResponseAdminDTO>> GetUsersAdmin(int page, string? name)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.User.Count() / pageResult);
            var users = await _context.User
                .Where(p => name == null || p.Username.Contains(name))
                .Include(p => p.ImageUsers)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .OrderBy(p => p.Followers)

                .ToListAsync();


            var response = new UserResponseAdminDTO
            {
                Users = _mapper.Map<List<UserDTO>>(users),
                CurrentPage = page,
                Pages = (int)pageCount

            };

            return response;


        }
        public EUser? ValidateUser(UserLoginDTO userLogin)
        {


            var user = _context.User.FirstOrDefault(p => p.Username == userLogin.Username || p.Email == userLogin.Username);


            if (user == null)
            {
                // El usuario no existe
                return null;
            }

            // Verificar la contraseña proporcionada con el hash almacenado
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password);

            if (!isPasswordValid)
            {
                // La contraseña es incorrecta
                return null;
            }

            // El usuario ha sido autenticado exitosamente
            return user;

        }

        public bool ValidateEmail(string email)
        {
            var user = _context.User.FirstOrDefault(p => p.Email == email);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public bool ValidateUsername(string username)
        {
            var user = _context.User.FirstOrDefault(p => p.Username == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public bool ValidatePassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            return hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);
        }

        public void AddUser(EUser user)
        {
            _context.User.Add(user);
        }

        public void SaveChange()
        {
            _context.SaveChanges();
        }

        public ActionResult<ImageUserDTO> GetImage(int id)
        {
            var Image = _context.ImageUser.FirstOrDefault(p => p.UserId == id);
            if (Image == null)
            {
                return null;
            }
            return _mapper.Map<ImageUserDTO>(Image);

        }
        public  ActionResult<ImageUserDTO> GetImageHeader(int userid)
        {
            var Image = _context.ImageHeader.FirstOrDefault(p => p.Id == 3);
            if (Image == null)
            {
                return _mapper.Map<ImageUserDTO>(Image);
            }
            return _mapper.Map<ImageUserDTO>(Image);

        }
        

        public EUser GetUser(int id)
        {
            return _context.User.Include(p => p.ImageUsers).Include(p => p.ImageUsersHeader)
                .FirstOrDefault(p => p.Id == id);
        }

        public async Task<EUser> RefreshToken(string refreshToken)
        {
            if(refreshToken == null)
            {
                return null;
            }
            var user = _context.User.FirstOrDefault(p => p.refreshToken == refreshToken);
            if (user == null)
            {
                return null;
            }

            if(user.TokenExpires < DateTime.Now)
            {
                return null;
            }
            return user;
        }

        public EFollowing GetFollow(int FollowerId, int FollowedId)
        {
            return _context.Following.FirstOrDefault(p => p.FollowerId == FollowerId && p.FollowedId == FollowedId);
        }

        public void Follow(EFollowing follow)
        {
             _context.Following.Add(follow);
        }

        public void UnFollow(EFollowing follow)
        {
            _context.Following.Remove(follow);
        }
        public  EUser VerifyEmail(string token)
        {
            var user = _context.User.FirstOrDefault(p => p.verifyToken == token);
            if (user == null)
            {
                throw new Exception("User not found");
            }
            user.Status = true;
            _context.User.Update(user);
            _context.SaveChanges();
            return user;
        }
        public bool SendVerify(string emailTo, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("FVSocial", "NAVAJAFINA"));
            message.To.Add(new MailboxAddress("Nombre Destinatario", emailTo));
            message.Subject = "Verify your account !";
            var builder = new BodyBuilder();
            builder.HtmlBody = @"
                <h1>Welcome to FVSocial</h1>
                <p>We've received a request to verify your account. Please proceed to verify your account by clicking the link below:</p>
                <a href='{your-link}" + token + @"' style='display: inline-block; background-color: blue; color: white; padding: 10px 20px; margin: 10px auto; text-align: center; text-decoration: none;'>Verify Account</a>
                <br>
                <img src='' alt='FVSocial' />
                <p> every post is a window to a world of connections, ideas, and shared moments. </p>
                <p>Join our community and discover a unique social interaction experience.</p>
                
                <p>Link: </p>
                 <a href='{your-link}" + token + @"'></a>

                ";

            message.Body = builder.ToMessageBody();

            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false); // Cambia esto según tu proveedor de correo
                    client.Authenticate("{your-email}", "{your-pass}"); // Utiliza la contraseña de aplicación
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                // Maneja los errores aquí, por ejemplo, registra el error en un archivo de registro
                return false;
            }
        }
        public async Task<UserResponseDTO> GetWhoToFollow( int id,int page)
        {
            var pageResult = 10f;
            var pageCount = Math.Ceiling(_context.User.Count() / pageResult);
            var users = await _context.User
                .Where(p => p.Id != id)
                .Include(p => p.ImageUsers)
                .Skip((page - 1) * (int)pageResult)
                .Take((int)pageResult)
                .OrderBy(p => p.Followers)
                .ToListAsync();

            var response = new UserResponseDTO
            {
                Users = _mapper.Map<List<UsersDTO>>(users),
                CurrentPage = page,
                Pages = (int)pageCount

            };

            return response;
        }

        public EUser GetUserByEmail(string email)
        {
            return _context.User.FirstOrDefault(p => p.Email == email);
        }
    }
}
