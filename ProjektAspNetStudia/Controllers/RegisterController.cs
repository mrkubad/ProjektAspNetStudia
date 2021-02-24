using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjektAspNetStudia.Models;
using ProjektAspNetStudia.Models.ViewModels;
using ProjektAspNetStudia.Utilities;

namespace ProjektAspNetStudia.Controllers
{
    public class RegisterController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Register(string language)
        {
            if (!CultureUtility.SetCurrentThreadCulture(language))
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string language, RegisterViewModel model)
        {
            if (!CultureUtility.SetCurrentThreadCulture(language))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userCreationResult = await _userManager.CreateAsync(AppUser.CreateFromRegisterViewModel(model), model.Password);

            if (userCreationResult.Succeeded)
            {
                return RedirectToAction("SignIn", "Login");
            }

            foreach (var error in userCreationResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
