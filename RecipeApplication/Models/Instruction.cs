using System.ComponentModel.DataAnnotations;

namespace RecipeApplication.Models
{
    public class Instruction
    {
        public int InstructionId { get; set; }

        [Required]
        public int StepNumber { get; set; }  // The step number must be defined

        [Required]
        public string Description { get; set; }  // The description must not be null

        // Foreign key to Recipe (non-nullable)
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }  // Navigation property
    }

}
