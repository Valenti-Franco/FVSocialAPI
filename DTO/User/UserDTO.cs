using SocialAPI.DTO.Image;
using SocialAPI.Entities;

namespace SocialAPI.DTO.User
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool Status { get; set; }

        public int Followers { get; set; }
        public int Following { get; set; }

        public ICollection<ImageUserDTO> ImageUsers { get; set; }

        public ICollection<ImageUserDTO> ImageUsersHeader { get; set; }


    }
}
