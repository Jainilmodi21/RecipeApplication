using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
namespace RecipeApplication.Models
{
    public class ApplicationUser: IdentityUser
    {
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>(); 
    }
}
