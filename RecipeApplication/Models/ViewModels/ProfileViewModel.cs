using System.ComponentModel.DataAnnotations;

namespace RecipeApplication.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        // Add other profile properties as needed, for example:

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
