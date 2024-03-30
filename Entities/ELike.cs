using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Entities
{
    public class ELike
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey("EUser")]
        public int UserId { get; set; }
        public EUser EUser { get; set; }


        [ForeignKey("EPost")]
        public int PostId { get; set; }
        public EPost EPost { get; set; }


    }
}
