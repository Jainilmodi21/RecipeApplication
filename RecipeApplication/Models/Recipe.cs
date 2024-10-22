using System.ComponentModel.DataAnnotations;

namespace RecipeApplication.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public string? ImageUrl { get; set; }  // Optional field, so it's nullable

        // Foreign key for the user who created the recipe (non-nullable)
        public string ApplicationUserId { get; set; }  // Make sure this matches your ApplicationUser primary key type

        // Navigation property
        public ApplicationUser ApplicationUser { get; set; }  // Navigation property

        public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
        public ICollection<Instruction> Instructions { get; set; } = new List<Instruction>();
    }

}
