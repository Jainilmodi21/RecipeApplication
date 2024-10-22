
using Microsoft.EntityFrameworkCore;
using RecipeApplication.Data;
using RecipeApplication.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApplication.Repositories
    {
        public interface IRecipeRepository
        {
            Task AddRecipe(Recipe recipe);
            Task UpdateRecipe(Recipe recipe);
            Task DeleteRecipe(Recipe recipe);
            Task<Recipe?> GetRecipeById(int id);
            Task<IEnumerable<Recipe>> GetRecipes();
        }

    public class RecipeRepository : IRecipeRepository
    {
        private readonly ApplicationDbContext _context;

        public RecipeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddRecipe(Recipe recipe)
        {
            _context.Recipes.Add(recipe); // Assuming 'Recipes' is the DbSet for the Recipe entity
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecipe(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRecipe(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
        }

        public async Task<Recipe?> GetRecipeById(int id) => await _context.Recipes
            .Include(r => r.Ingredients) // Include related Ingredients
            .Include(r => r.Instructions) // Include related Instructions
            .FirstOrDefaultAsync(r => r.RecipeId == id);

        public async Task<IEnumerable<Recipe>> GetRecipes() => await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Instructions)
            .ToListAsync();
    }
}

