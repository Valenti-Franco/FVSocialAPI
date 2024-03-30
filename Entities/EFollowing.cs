using System.ComponentModel.DataAnnotations.Schema;

namespace SocialAPI.Entities
{
    public class EFollowing
    {
        public int Id { get; set; }

        [ForeignKey("Follower")]
        public int FollowerId { get; set; }
        public EUser Follower { get; set; }

        [ForeignKey("Followed")]
        public int FollowedId { get; set; }
        public EUser Followed { get; set; }

    }
}
