namespace SocialAPI.Entities
{
    public class EImageHeader
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string publicId { get; set; }
        public int UserId { get; set; }
        public EUser User { get; set; }
    }
}
