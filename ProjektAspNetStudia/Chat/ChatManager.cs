using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjektAspNetStudia.Models;
using ProjektAspNetStudia.Models.Database;
using ProjektAspNetStudia.Views.Shared.Components.RecentChat;

namespace ProjektAspNetStudia.Chat
{
    public class ChatManager
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        public ChatManager(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }
        public async Task<List<Message>> GetChatMessages(string userId)
        {
            var result = new List<Message>();

            if (string.IsNullOrWhiteSpace(userId))
            {
                Debug.WriteLine("UserId is null or empty...");
                return result;
            }

            var chatId = _dbContext.Users.FirstOrDefault(u => u.Id == userId)?.CurrentChatId;

            if (!chatId.HasValue)
            {
                Debug.WriteLine("Couldn't find the chat value for user.");
                return result;
            }

            var chat = await _dbContext.Chats.FirstOrDefaultAsync(ch => ch.ChatId == chatId.Value);

            if (chat is null) // when there is no chat with that id
            {
                Debug.WriteLine("Couldn't find the chat with specified id.");
                return result;
            }

            var isAuthorized = await _dbContext.ChatUsers.FirstOrDefaultAsync(chu => chu.AppUser.Id == userId && chu.Chat == chat);

            if (isAuthorized is null)
            {
                Debug.WriteLine("User isn't authorized for given chat.");
                return result;
            }


            var messages = _dbContext.Messages.Where(m => m.Chat == chat && m.Text != null);
            result.AddRange(messages);

            return result;
        }

        public int? GetChatIdBasedOnClaimsPrincipal(ClaimsPrincipal user)
        {
            var userId = _userManager.GetUserId(user);

            if (string.IsNullOrWhiteSpace(userId))
            {
                Debug.WriteLine("UserId is null or empty...");
                return null;
            }

            var chatId = _dbContext.Users.FirstOrDefault(u => u.Id == userId)?.CurrentChatId;

            return chatId;
        }

        public async Task<List<RecentChatModel>> GetChatsForGivenUser(ClaimsPrincipal user)
        {
            var result = new List<RecentChatModel>();

            var userObj = await _userManager.GetUserAsync(user);

            if (userObj is null)
            {
                Debug.WriteLine("User not found...");
                return result;
            }

            var chatUserRelations = _dbContext.ChatUsers.Where(chu => chu.AppUser == userObj);


            foreach (var chatRelation in chatUserRelations)
            {
                var chatRelationEntry = _dbContext.Entry(chatRelation);

                var chatQuery = chatRelationEntry.Reference(chu => chu.Chat).Query();

                var chat = await chatQuery.FirstOrDefaultAsync();

                var lastMessageQuery = _dbContext.Messages.Where(m => m.Chat == chat).OrderByDescending(m => m.Sent).Take(1);
                var lastMessage = await lastMessageQuery.FirstOrDefaultAsync();

                if (chat is null)
                {
                    continue;
                }

                result.Add(new RecentChatModel
                {
                    IsActive = userObj.CurrentChatId == chat.ChatId,
                    ChatName = chat.ChatName,
                    ChatId = chat.ChatId,
                    IsChatRead = chatRelation.IsChatRead,
                    LastModification = chat.LastModificationTime,
                    LastMessageDate = lastMessage?.Sent.ToString("MMM dd") ?? string.Empty,
                    LastMessageText = lastMessage?.Text ?? string.Empty
                });
            }
            return result.OrderByDescending(m => m.LastModification).ToList();
        }

    }
}
