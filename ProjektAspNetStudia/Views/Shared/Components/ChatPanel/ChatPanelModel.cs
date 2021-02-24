using System.Collections.Generic;
using ProjektAspNetStudia.Models.Database;

namespace ProjektAspNetStudia.Views.Shared.Components.ChatPanel
{
    public class ChatPanelModel
    {
        public List<Message> Messages { get; set; }
        public string UserId { get; set; }
    }
}
