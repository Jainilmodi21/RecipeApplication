using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeApplication.Models;

namespace RecipeApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Instruction> Instructions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>()
                .HasOne(r => r.ApplicationUser)   // Each Recipe has one ApplicationUser
                .WithMany(u => u.Recipes)          // Each ApplicationUser can have many Recipes
                .HasForeignKey(r => r.ApplicationUserId)  // FK is ApplicationUserId (string)
                .IsRequired();  // Optionally enforce that ApplicationUserId is required
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
