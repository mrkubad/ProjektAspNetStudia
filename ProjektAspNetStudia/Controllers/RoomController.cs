using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjektAspNetStudia.Models;
using ProjektAspNetStudia.Models.Database;

namespace ProjektAspNetStudia.Controllers
{
    public class RoomCreationModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("users")]
        public List<string> SelectedUsers { get; set; }
    }
    public class RoomController: Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        public RoomController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("/api/room/create")]
        public async Task<IActionResult> CreateRoom([FromBody]RoomCreationModel model)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }

            var userWhichRequestedCreation = await _userManager.GetUserAsync(HttpContext.User);

            var newChat = new Models.Database.Chat
            {
                LastModificationTime = DateTime.Now,
                ChatName = model.Name,
                ChatUsers = new List<ChatUser>()
            };

            foreach (var selectedUser in model.SelectedUsers)
            {
                var user = await _dbContext.Users.FirstAsync(u => u.Id == selectedUser);
                newChat.ChatUsers.Add(new ChatUser
                {
                    Chat = newChat,
                    AppUser = user
                });
            }


            if (!newChat.ChatUsers.Any(chu => chu.AppUser == userWhichRequestedCreation && chu.Chat == newChat))
            {
                newChat.ChatUsers.Add(new ChatUser
                {
                    Chat = newChat,
                    AppUser = userWhichRequestedCreation
                });
            }

            await _dbContext.Chats.AddAsync(newChat);

            await _dbContext.SaveChangesAsync();


            return Ok();
        }
    }
}
