namespace ProjektAspNetStudia.Models.Database
{
    public class ChatUser
    {
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public string UserId { get; set; }
        public AppUser AppUser { get; set; }
        public bool IsChatRead { get; set; }
    }
}