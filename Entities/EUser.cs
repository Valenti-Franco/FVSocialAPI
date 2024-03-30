
using System.ComponentModel.DataAnnotations;

namespace SocialAPI.Entities
{
    public class EUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "The Username field is required.")]
        [MaxLength(40)]
        public string Username { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Email field is required.")]
        [EmailAddress(ErrorMessage = "The Email field must be a valid email address.")]
        [MaxLength(100)]

        public string Email { get; set; }

    
        public string Password { get; set; } = string.Empty;


        public string Role { get; set; } = "User";

        public Boolean IsBanned { get; set; } = false;

        public Boolean Status { get; set; } = false;

        public int Followers { get; set; } = 0;

        public int Following { get; set; } = 0;

        [MaxLength(200)]
        public string Bio { get; set; } = string.Empty;

        public int PostsCount { get; set; } = 0;

        public int PinPost { get; set; } = 0;

        public string Link1 { get; set; } = string.Empty;
        public string Link2 { get; set; } = string.Empty;
        [MaxLength(30)]
        public string Location { get; set; } = string.Empty;

        public Boolean isPrivate { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public DateTime DeletedAr { get; set; } 

        public string verifyToken { get; set; } = string.Empty;

        public string resetPassToken { get; set; } = string.Empty;

        public string refreshToken { get; set; } = string.Empty;

        public DateTime TokenCreated { get; set; } 

        public DateTime TokenExpires { get; set; }
        public ICollection<EPost> Posts { get; set; }

        public ICollection<ELike> Likes { get; set; }

        public ICollection<EComment> Comments { get; set; }

        public ICollection<EImageUser> ImageUsers { get; set; }

        public ICollection<EImageHeader> ImageUsersHeader { get; set; }



        public EUser(string username)
        {
            Username = username;
        }

    }
}
