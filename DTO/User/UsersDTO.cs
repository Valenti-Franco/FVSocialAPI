using SocialAPI.DTO.Image;
using SocialAPI.Entities;

namespace SocialAPI.DTO.User
{
    public class UsersDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public ICollection<ImageUserDTO> ImageUsers { get; set; }
    }
}
