using System.ComponentModel.DataAnnotations;

namespace RecipeApplication.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Quantity { get; set; }

        // Foreign key to Recipe (non-nullable)
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }  // Navigation property
    }

}
