using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pizza_API.Entities;

namespace Pizza_API.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Define roles
            string[] roles = new[] { "Admin", "Guest" };

            // Seed roles
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();

            if (!environment.IsDevelopment())
            {
                return;
            }

            // Seed development admin user
            var adminEmail = "admin@pizzashop.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System Administrator",
                    LastName = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin@pizzashop123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }

        public static async Task SeedCategoriesAsync(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Copy seed images to uploads folder if they don't exist
            var seedDir = Path.Combine(env.WebRootPath, "seeds", "categories");
            var uploadDir = Path.Combine(env.WebRootPath, "uploads", "categories");

            Directory.CreateDirectory(uploadDir);

            foreach (var file in Directory.GetFiles(seedDir))
            {
                var dest = Path.Combine(uploadDir, Path.GetFileName(file));
                if (!File.Exists(dest))
                    File.Copy(file, dest);
            }

            // Then seed the DB
            if (await context.Categories.AnyAsync())
                return;

            var categories = new List<Category>
            {
                new Category { Name = "Pizzas",  ImagePath = "/uploads/categories/pizzaCategory.png",  ImageFileName = "pizzaCategory.png"  },
                new Category { Name = "Kebabs",  ImagePath = "/uploads/categories/kebabCategory.png",  ImageFileName = "kebabCategory.png"  },
                new Category { Name = "Salads",  ImagePath = "/uploads/categories/saladCategory.png",  ImageFileName = "saladCategory.png"  },
                new Category { Name = "Sides",   ImagePath = "/uploads/categories/sidesCategory.png",  ImageFileName = "sidesCategory.png"  },
                new Category { Name = "Drinks",  ImagePath = "/uploads/categories/drinkCategory.png", ImageFileName = "drinkCategory.png" },
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        public static void SeedDefaultImages(IWebHostEnvironment env)
        {
            var defaultSeedDir = Path.Combine(env.WebRootPath, "seeds", "default");
            var defaultUploadDir = Path.Combine(env.WebRootPath, "uploads", "default");

            Directory.CreateDirectory(defaultUploadDir);

            foreach (var file in Directory.GetFiles(defaultSeedDir))
            {
                var dest = Path.Combine(defaultUploadDir, Path.GetFileName(file));
                if (!File.Exists(dest))
                    File.Copy(file, dest);
            }
        }
    }
}
