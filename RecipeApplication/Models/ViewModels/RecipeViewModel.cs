using System.ComponentModel.DataAnnotations;

namespace RecipeApplication.Models.ViewModels
{
    public class RecipeViewModel
    {
        public int RecipeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;  // Title cannot be null

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;  // Description cannot be null

        [Url]
        public string? ImageUrl { get; set; }  // Nullable because the user may not provide an image URL

        // Add IFormFile to handle image upload
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }  // Nullable, as the user may not upload an image



        public string? ApplicationUserId { get; set; }  // Nullable, user might not always be provided

        // Add collections for ingredients and instructions
        public List<IngredientViewModel> Ingredients { get; set; } = new List<IngredientViewModel>();

        public List<InstructionViewModel> Instructions { get; set; } = new List<InstructionViewModel>();
    }

    public class IngredientViewModel
    {
        public int IngredientId { get; set; } // Unique identifier for the ingredient

        [Required]
        public string Name { get; set; } = null!;  // Name cannot be null

        public string? Quantity { get; set; }  // Nullable, as a user may choose not to enter a quantity

        public bool IsDeleted { get; set; }  // Mark for deletion (boolean cannot be null, default is false)
    }

    public class InstructionViewModel
    {
        public int InstructionId { get; set; } // Unique identifier for the instruction

        public string? Step { get; set; }  // Nullable, as some instructions may not have a detailed step description

        public bool IsDeleted { get; set; }  // Mark for deletion (boolean cannot be null, default is false)
    }
}
