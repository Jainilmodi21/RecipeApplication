using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RecipeApplication.Models;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using RecipeApplication.Data;
using RecipeApplication.Models.ViewModels;
using System.Security.Claims;
using RecipeApplication.Repositories;


namespace RecipeApplication.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IRecipeRepository _recipeRepository;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IRecipeRepository recipeRepository)
        {
            _logger = logger;
            _context = context;
            _recipeRepository = recipeRepository;
        }

        // GET: /Recipes
        public async Task<IActionResult> Index()  // Renamed from List to Index
        {
            var recipes = await _context.Recipes.ToListAsync();
            return View(recipes);
        }

        public async Task<IActionResult> MyRecipes()
        {
            // Get the ID of the currently logged-in user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch recipes only created by the logged-in user
            var recipes = await _context.Recipes
                                        .Where(r => r.ApplicationUserId == userId)
                                        .ToListAsync();

            return View(recipes);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

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
                Instructions = new List<InstructionViewModel>(),
                ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecipeViewModel model, IFormFile ImageFile)
        {
            // If the model state is invalid, log the errors and return the same view
            if (!ModelState.IsValid)
            {
                // Log the model state errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"Validation Error: {error.ErrorMessage}"); // Log to console or handle as needed
                }
                return View(model); // Return the form with validation errors
            }

            // If ModelState is valid, proceed with creating the Recipe entity
            if (ModelState.IsValid)
            {
                string imagePath = null;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // Get the file extension of the uploaded file
                    var extension = Path.GetExtension(model.ImageFile.FileName);
                    // Generate a unique file name to prevent overwriting
                    var fileName = Guid.NewGuid().ToString() + extension;
                    // Define the file path to save the image in the ~/wwwroot/images/ folder
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                    // Save the file to the specified path
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Store the image URL relative to the wwwroot folder
                    model.ImageUrl = "/images/" + fileName;
                }

                // Create the recipe entity using data from the view model
                var recipe = new Recipe
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Retrieve the logged-in user's ID

                    // Map ingredients, excluding those marked for deletion
                    Ingredients = model.Ingredients
                        .Where(i => !i.IsDeleted) // Only include ingredients that are not marked for deletion
                        .Select(i => new Ingredient
                        {
                            Name = i.Name,
                            Quantity = i.Quantity,
                            RecipeId = model.RecipeId // Ensure RecipeId is set for each ingredient
                        })
                        .ToList(),

                    // Map instructions, excluding those marked for deletion
                    Instructions = model.Instructions.Where(i => !i.IsDeleted) // Only include instructions that are not marked for deletion
    .Select((i, index) => new Instruction
    {
        StepNumber = index + 1, // StepNumber is set to incremental values (1, 2, 3, ...)
        Description = i.Step,
        RecipeId = model.RecipeId // Ensure RecipeId is set for each instruction
    })
    .ToList()

                };

                // Add the recipe to the context and save changes
                _context.Recipes.Add(recipe); // Add the recipe entity to the Recipes DbSet
                await _context.SaveChangesAsync(); // Persist changes to the database

                // Redirect to the Index page or any other page after successful creation
                return RedirectToAction(nameof(Index));
            }

            // If something goes wrong, return the model back to the view
            return View(model);
        }

        public async Task<IActionResult> Search(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return View("Index", await _context.Recipes.ToListAsync());  // Or return an empty view if needed
            }

            // Find recipe by name (or adjust the search condition as needed)
            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(r => r.Title.Contains(searchString));

            if (recipe == null)
            {
                return View("NotFound");  // Optional: Handle the case when the recipe isn't found
            }

            // Redirect to the detail page of the found recipe
            return RedirectToAction("Detail", new { id = recipe.RecipeId});
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
                Ingredients = recipe.Ingredients.Select(i => new IngredientViewModel { IngredientId = i.IngredientId, Name = i.Name, Quantity=i.Quantity }).ToList(),
                Instructions = recipe.Instructions.Select(i => new InstructionViewModel { InstructionId=i.InstructionId, Step = i.Description }).ToList()
            };

            return View(model);
        }

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
                    // Retrieve the existing recipe with its ingredients and instructions
                    var recipe = await _context.Recipes
                        .Include(r => r.Ingredients)
                        .Include(r => r.Instructions)
                        .FirstOrDefaultAsync(r => r.RecipeId == id);

                    if (recipe == null)
                    {
                        return NotFound();
                    }

                    // Update basic fields
                    recipe.Title = model.Title;
                    recipe.Description = model.Description;
                    //recipe.ImageUrl = model.ImageUrl;
                    _context.SaveChanges();

                    // Handle Ingredients
                    foreach (var ingredient in model.Ingredients)
                    {
                        if (ingredient.IsDeleted)
                        {
                            // Remove ingredient if marked for deletion
                            var toRemove = recipe.Ingredients.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId);
                            _context.SaveChanges();
                            if (toRemove != null)
                            {
                                _context.Ingredients.Remove(toRemove);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            if (ingredient.IngredientId == 0)
                            {
                                // Add new ingredient
                                recipe.Ingredients.Add(new Ingredient
                                {
                                    Name = ingredient.Name,
                                    Quantity = ingredient.Quantity,
                                    RecipeId = recipe.RecipeId
                                });
                                _context.SaveChanges();
                            }
                            else
                            {
                                // Update existing ingredient
                                var existingIngredient = recipe.Ingredients.FirstOrDefault(i => i.IngredientId == ingredient.IngredientId);
                                if (existingIngredient != null)
                                {
                                    existingIngredient.Name = ingredient.Name;
                                    existingIngredient.Quantity = ingredient.Quantity;

                                    _context.SaveChanges();
                                }
                            }
                        }
                    }

                    // Handle Instructions
                    foreach (var instruction in model.Instructions)
                    {
                        if (instruction.IsDeleted)
                        {
                            // Remove instruction if marked for deletion
                            var toRemove = recipe.Instructions.FirstOrDefault(i => i.InstructionId == instruction.InstructionId);
                            _context.SaveChanges();
                            if (toRemove != null)
                            {
                                _context.Instructions.Remove(toRemove);
                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            if (instruction.InstructionId == 0)
                            {

                                // Add new instruction
                                recipe.Instructions.Add(new Instruction
                                {
                                    StepNumber = recipe.Instructions.Count + 1,
                                    Description = instruction.Step,
                                    RecipeId = recipe.RecipeId
                                });
                                _context.SaveChanges();
                            }
                            else
                            {
                                // Update existing instruction
                                var existingInstruction = recipe.Instructions.FirstOrDefault(i => i.InstructionId == instruction.InstructionId);
                                if (existingInstruction != null)
                                {
                                    existingInstruction.Description = instruction.Step;
                                    _context.SaveChanges();
                                    
                                }
                            }
                        }
                    }

                    // Save the changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Redirect to the Index after successful update
                return RedirectToAction(nameof(Index));
            }

            // Return the same view if model state is invalid
            return View(model);
        }

        // Helper method to check if a recipe exists
        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.RecipeId == id);
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
