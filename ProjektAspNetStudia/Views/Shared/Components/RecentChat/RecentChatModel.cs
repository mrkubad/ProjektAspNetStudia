using System;

namespace ProjektAspNetStudia.Views.Shared.Components.RecentChat
{
    public class RecentChatModel
    {
        public string ChatName { get; set; }
        public DateTime LastModification { get; set; }
        public bool IsChatRead { get; set; }
        public string LastMessageText { get; set; }
        public string LastMessageDate { get; set; }
        public bool IsActive { get; set; }
        public int ChatId { get; set; }
    }
}
