using System;
using System.Collections.Generic;

namespace ProjektAspNetStudia.Models.Database
{
    public class Chat
    {
        public int ChatId { get; set; }
        public string ChatName { get; set; }
        public DateTime LastModificationTime { get; set; }
        public ICollection<ChatUser> ChatUsers { get; set; }
        public ICollection<Message> Messages { get; set; }
    }
}