using System;

namespace ProjektAspNetStudia.Models.Database
{
    public class Message
    {
        public int MessageId { get; set; }
        public string Text { get; set; }
        public string SentBy { get; set; }
        public DateTime Sent { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}