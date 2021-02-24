using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using ProjektAspNetStudia.Models;
using ProjektAspNetStudia.Models.Json;

namespace ProjektAspNetStudia.Controllers
{
    public class SearchController : Controller
    {
        private readonly Regex _regex;
        private readonly AppDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public SearchController(AppDbContext dbContext, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _regex = new Regex("^[a-zA-Z0-9' ]+$");
        }

        [HttpGet]
        [Produces("application/json")]
        [Route("/api/users/search")]
        public IActionResult Index(string q)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return StatusCode(403);
            }

            var result = new SearchModel
            {
                Data = new List<SearchResult>()
            };

            if (string.IsNullOrWhiteSpace(q) || !_regex.IsMatch(q))
            {
                return Json(result);
            }

            var userWhoRequested = _userManager.GetUserId(HttpContext.User);

            var subStrToFind = q.ToLowerInvariant();

            var userQuery = _dbContext.Users.Where(u =>
                u.LastName.Contains(subStrToFind) ||
                u.FirstName.Contains(subStrToFind)).Where(u => u.Id != userWhoRequested).Take(10);

            foreach (var user in userQuery)
            {
                var userName = string.Concat(user.FirstName, " ", user.LastName);

                var searchResult = new SearchResult
                {
                    UserId = user.Id,
                    UserName = userName
                };

                result.Data.Add(searchResult);
            }

            return Json(result);
        }
    }
}