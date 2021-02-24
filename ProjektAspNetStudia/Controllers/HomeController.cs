using Microsoft.AspNetCore.Mvc;
using ProjektAspNetStudia.Models;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProjektAspNetStudia.Models.ViewModels;
using ProjektAspNetStudia.Utilities;

namespace ProjektAspNetStudia.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _dbContext;

        public HomeController(UserManager<AppUser> userManager, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string language)
        {
            if (!CultureUtility.SetCurrentThreadCulture(language))
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Chat", "Home", new {chatId = user.CurrentChatId});
            }
            return Redirect($"/{language}/login/signin");
        }
        
        [Route("{language=en-US}/Home/Chat/{chatId?}")]
        public async Task<IActionResult> Chat(string language, int? chatId = null)
        {
            if (!CultureUtility.SetCurrentThreadCulture(language))
            {
                return NotFound();
            }

            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }

            if (chatId.HasValue)
            {
                var userId = _userManager.GetUserId(HttpContext.User);

                var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    user.CurrentChatId = chatId.Value;
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    return Error();
                }
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
