using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Entities
{
    public class EComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public DateTime DeletedAr { get; set; }


        [ForeignKey("EUser")]
        public int UserId { get; set; }
        public EUser User { get; set; }



        [ForeignKey("EPost")]
        public int PostId { get; set; }
        public EPost EPost { get; set; }


        public EComment(string content)
        {
            Content = content;
        }
    }
}
