using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektAspNetStudia.Models;
using ProjektAspNetStudia.Models.ViewModels;
using ProjektAspNetStudia.Utilities;

namespace ProjektAspNetStudia.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public LoginController(AppDbContext dbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public IActionResult SignIn(string language)
        {
            if (!CultureUtility.SetCurrentThreadCulture(language))
            {
                return NotFound();
            }
            return View();
        }

        public IActionResult HaveAccount()
        {
            return RedirectToAction("Register", "Register");
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }


            var user = await _userManager.FindByEmailAsync(login.Email);

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, login.Password);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("CustomErrorField", "Invalid credentials");
                return View(login);
            }

            var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);


            if (result.Succeeded)
            {
                return RedirectToAction("Chat", "Home", new {chatId = user.CurrentChatId});
            }
          
            ModelState.AddModelError("CustomErrorField", "Invalid credentials.");
            return View(login);
        }

        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn");
        }
    }
}
