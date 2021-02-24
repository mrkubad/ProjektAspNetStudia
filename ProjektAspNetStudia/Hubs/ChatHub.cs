using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ProjektAspNetStudia.Models;
using ProjektAspNetStudia.Models.Database;
using ProjektAspNetStudia.Utilities;

namespace ProjektAspNetStudia.Hubs
{
    public class ChatHub: Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        private static readonly Dictionary<string, string> Connections = new Dictionary<string, string>();

        public ChatHub(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public override Task OnConnectedAsync()
        {
            var user = _userManager.GetUserId(Context.User);
            System.Diagnostics.Debug.WriteLine($"UserId z huba: {user}");

            if (user is null)
            {
                Context.Abort();
                return Task.CompletedTask;
            }


            if (Connections.ContainsKey(user))
            {
                Connections[user] = Context.ConnectionId;
            }
            else
            {
                Connections.Add(user, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var user = _userManager.GetUserId(Context.User);

            if (user != null && Connections.ContainsKey(user))
            {
                Connections.Remove(user);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public Task MarkChatAsRead(string chatId) => ChangeChatReadState(int.Parse(chatId), true);
        public Task MarkChatAsNotRead(string chatId) => ChangeChatReadState(int.Parse(chatId), false);
        private async Task ChangeChatReadState(int chatId, bool state)
        {
            var userId = _userManager.GetUserId(Context.User);

            var result = await _dbContext.ChatUsers.FirstOrDefaultAsync(chu => chu.ChatId == chatId && chu.UserId == userId);

            if (result is null)
            {
                return;
            }

            result.IsChatRead = state;

            await _dbContext.SaveChangesAsync();
        }

        public async Task SendMessage(string message)
        {

            var userId = _userManager.GetUserId(Context.User);

            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);
            var chatId = user?.CurrentChatId;

            if (!chatId.HasValue)
            {
                return;
            }

            var chat = await _dbContext.Chats.FirstOrDefaultAsync(ch => ch.ChatId == chatId);
            if (chat is null)
            {
                return;
            }
            chat.LastModificationTime = DateTime.Now;
            var messageObj = new Message
            {
                Chat = chat,
                Sent = DateTime.Now,
                Text = message,
                SentBy = userId
            };

            await _dbContext.Messages.AddAsync(messageObj);

            await _dbContext.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine(message);


            var chatUsers = _dbContext.ChatUsers.Where(chu => chu.Chat == chat);

            foreach (var chatUser in chatUsers)
            {
                var userObj = await _dbContext.Users.FirstOrDefaultAsync(usr => usr.Id == chatUser.UserId);

                if (userObj == null)
                {
                    continue;
                }

                if (userObj.CurrentChatId == chatId.Value)
                {
                    // ADD LOGIC FOR USER, WHICH SENDS THE MESSAGE
                    if (Connections.ContainsKey(userObj.Id))
                    {
                        await Clients.Client(Connections[userObj.Id]).SendAsync("ReceiveMessage", messageObj.CreateMessageForJsClient(string.Concat(user.FirstName, " ", user.LastName)), userObj.Id == userId);
                    }
                    continue;
                }

                if (Connections.ContainsKey(userObj.Id))
                {
                    await Clients.Client(Connections[userObj.Id]).SendAsync("ReceiveNotification", messageObj.CreateMessageForJsClient(string.Empty), chat.ChatId); // send message and new chat id
                }
            }
        }
    }
}
