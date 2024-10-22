using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeApplication.Models;
using System.Threading.Tasks;
using RecipeApplication.Models.ViewModels;
using RecipeApplication.Data;

namespace Recipe_Sharing.Controllers
{
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecipesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Recipes
        public async Task<IActionResult> Index()  // Renamed from List to Index
        {
            var recipes = await _context.Recipes.ToListAsync();
            return View(recipes);
        }

        // GET: /Recipes/Detail/5
        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var recipe = await _context.Recipes
                .Include(r => r.Ingredients)    // Eagerly load the Ingredients collection
                .Include(r => r.Instructions)   // Eagerly load the Instructions collection
                .FirstOrDefaultAsync(m => m.RecipeId == id);

            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }




        // GET: Recipes/Create
        public IActionResult Create()
        {
            var model = new RecipeViewModel
            {
                Ingredients = new List<IngredientViewModel>(),
                Instructions = new List<InstructionViewModel>()
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var recipe = new Recipe
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    ApplicationUserId = model.ApplicationUserId,
                    Ingredients = model.Ingredients.Select(i => new Ingredient { Name = i.Name }).ToList(),
                    Instructions = model.Instructions.Select(i => new Instruction { Description = i.Step }).ToList(),
                };

                _context.Add(recipe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }


      // GET: Recipes/Edit/5
public async Task<IActionResult> Edit(int id)
{
    var recipe = await _context.Recipes
        .Include(r => r.Ingredients)
        .Include(r => r.Instructions)
        .FirstOrDefaultAsync(r => r.RecipeId == id);

    if (recipe == null)
    {
        return NotFound();
    }

    var model = new RecipeViewModel
    {
        RecipeId = recipe.RecipeId,
        Title = recipe.Title,
        Description = recipe.Description,
        ImageUrl = recipe.ImageUrl,
        Ingredients = recipe.Ingredients.Select(i => new IngredientViewModel { Name = i.Name }).ToList(),
        Instructions = recipe.Instructions.Select(i => new InstructionViewModel { Step = i.Description}).ToList()
    };

    return View(model);
}


        // POST: /Recipes/Edit/5
        // POST: /Recipes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecipeViewModel model)
        {
            if (id != model.RecipeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var recipe = await _context.Recipes.FindAsync(id);
                    if (recipe == null)
                    {
                        return NotFound();
                    }

                    // Map ViewModel fields to the Recipe entity
                    recipe.Title = model.Title;
                    recipe.Description = model.Description;
                    recipe.ImageUrl = model.ImageUrl;

                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Recipes.Any(r => r.RecipeId == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));  // Redirect to Index instead of List
            }

            return View(model);  // Return the ViewModel to the view if the ModelState is not valid
        }


        // GET: /Recipes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        // POST: /Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));  // Redirect to Index instead of List
        }
    }
}
