using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Entities
{
    public class EPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Content { get; set; }

        public int LikesCount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public DateTime DeletedAr { get; set; }

        [ForeignKey("EUser")]
        public int UserId { get; set; }
        public EUser User { get; set; }



        public ICollection<EComment> Comments { get; set; }

        public ICollection<ELike> Likes { get; set; }

        public ICollection<EImagePost> ImagePosts { get; set; }

        public EPost(string content)
        {
            Content = content;
        }
    }
}
