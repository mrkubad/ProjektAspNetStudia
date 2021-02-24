using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProjektAspNetStudia.Chat;

namespace ProjektAspNetStudia.Views.Shared.Components.RecentChat
{
    public class RecentChatViewComponent: ViewComponent
    {
        private readonly ChatManager _chatManager;

        public RecentChatViewComponent(ChatManager chatManager)
        {
            _chatManager = chatManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await _chatManager.GetChatsForGivenUser(HttpContext.User);
            return View(result);
        }
    }
}
