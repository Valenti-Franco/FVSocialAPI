using System.ComponentModel.DataAnnotations;

namespace SocialAPI.DTO.User
{
    public class UserCreateDTO
    {

        [Required(ErrorMessage = "The Username field is required.")]
        [MaxLength(50, ErrorMessage = "The Username field must be at most 50 characters.")]
        [MinLength(5, ErrorMessage = "The Username field must be at least 5 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The Password field is required.")]
        [MinLength(8, ErrorMessage = "The Password field must be at least 8 characters.")]
        public string Password { get; set; }


        public string Name { get; set; }
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "The Email field must be a valid email address.")]
        public string Email { get; set; }
    }
}
