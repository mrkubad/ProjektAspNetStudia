using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ProjektAspNetStudia.Models.Database;
using ProjektAspNetStudia.Models.ViewModels;

namespace ProjektAspNetStudia.Models
{
    public class AppUser: IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CurrentChatId { get; set; }
        public ICollection<ChatUser> ChatsUsers { get; set; }

        public static AppUser CreateFromRegisterViewModel(RegisterViewModel registerViewModel)
        {
            return new AppUser
            {
                UserName = $"{registerViewModel.FirstName}.{registerViewModel.LastName}",
                Email = registerViewModel.Email, 
                FirstName = registerViewModel.FirstName,
                LastName = registerViewModel.LastName
            };
        }
    }
}
