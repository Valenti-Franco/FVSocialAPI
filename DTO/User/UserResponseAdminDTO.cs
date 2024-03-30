namespace SocialAPI.DTO.User
{
    public class UserResponseAdminDTO
    {
        public List<UserDTO> Users { get; set; }
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
