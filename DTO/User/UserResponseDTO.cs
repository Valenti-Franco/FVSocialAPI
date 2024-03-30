namespace SocialAPI.DTO.User
{
    public class UserResponseDTO
    {
        public List<UsersDTO> Users { get; set; }
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
    }
}
