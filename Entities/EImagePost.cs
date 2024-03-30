namespace SocialAPI.Entities
{
    public class EImagePost
    {
        public int Id { get; set; }
        public string Image { get; set; }

        public string publicId { get; set; }
        public int PostId { get; set; }
        public EPost Post { get; set; }
    }
}
