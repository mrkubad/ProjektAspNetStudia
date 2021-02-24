using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektAspNetStudia.Chat;
using ProjektAspNetStudia.Models;

namespace ProjektAspNetStudia.Views.Shared.Components.ChatPanel
{
    public class ChatPanelViewComponent: ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ChatManager _chatManager;

        public ChatPanelViewComponent(UserManager<AppUser> userManager, ChatManager chatManager)
        {
            _chatManager = chatManager;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            System.Diagnostics.Debug.WriteLine($"UserId z componentu: {userId}");
            var messages = await _chatManager.GetChatMessages(userId);
            return View(new ChatPanelModel {Messages = messages, UserId = userId});
        }
    }
}
