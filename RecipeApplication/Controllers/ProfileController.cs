using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeApplication.Models;
using System.Threading.Tasks;
using RecipeApplication.Models.ViewModels;  // Ensure this line is added


namespace RecipeApplication.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: /Profile
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var profile = new ProfileViewModel
            {
                Email = user.Email
                // Add other profile information as needed
            };

            return View(profile);
        }
    }
}
