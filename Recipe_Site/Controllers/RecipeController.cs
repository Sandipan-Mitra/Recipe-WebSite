using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Recipe_Site.Models;

namespace Recipe_Site.Controllers
{
    public class RecipeController : Controller
    {
        ProjectContext db;
        IWebHostEnvironment env;
        public RecipeController(ProjectContext _db, IWebHostEnvironment _env)
        {
            db = _db;
            env = _env;
        }
        public async Task<IActionResult> Recipes(string search)
        {
            var emailid = HttpContext.Session.GetString("EmailID");
            List<Recipe> List;
            ViewBag.search = search;
            if (!string.IsNullOrEmpty(emailid))
            {
                if (string.IsNullOrEmpty(search))
                {
                    List = await db.tblRecipes.ToListAsync();
                }
                else
                {
                    List = await db.tblRecipes.Where(p => p.RecipeTitle.Contains(search)).ToListAsync();
                }
                return View(List);
            }
            else
            {
                return RedirectToAction("Login", "Users");
            }

        }
        [HttpGet]
        public IActionResult AddRecipe()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddRecipe(Recipe recipe)
        {
            try
            {
                var emailid = HttpContext.Session.GetString("EmailID");
                recipe.Ingredients = recipe.Ingredients.Replace("\r", "");
                recipe.Instructions = recipe.Instructions.Replace("\r", "");

                if (recipe.Difficulty == "--Select difficulty--")
                {
                    ModelState.AddModelError("Difficulty", "Please select the difficulty level!");
                    return View();
                }
                if (!string.IsNullOrEmpty(emailid))
                {
                    recipe.EmailID = emailid;
                }
                else
                {
                    return RedirectToAction("Login", "Users");
                }
                recipe.FoodImage = UploadFile(recipe.ImageFile);
                await db.AddAsync(recipe);
                await db.SaveChangesAsync();
                return RedirectToAction("Recipes");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                return View();
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditRecipe(int id)
        {
            var rowdetails = await db.tblRecipes.FirstOrDefaultAsync(p => p.RecipeID == id);
            return View(rowdetails);
        }
        [HttpPost]
        public async Task<IActionResult> EditRecipe(Recipe recipe)
        {
            try
            {
                var emailid = HttpContext.Session.GetString("EmailID");
                db.Entry(recipe).State = EntityState.Modified;
                recipe.Ingredients = recipe.Ingredients.Replace("\r", "");
                recipe.Instructions = recipe.Instructions.Replace("\r", "");

                if (recipe.Difficulty == "--Select difficulty--")
                {
                    ModelState.AddModelError("Difficulty", "Please select the difficulty level!");
                    return View();
                }
                if (!string.IsNullOrEmpty(emailid))
                {
                    recipe.EmailID = emailid;
                }
                else
                {
                    return RedirectToAction("Login", "Users");
                }
                if (recipe.ImageFile != null)
                {
                    var Imgpath = Path.Combine(env.WebRootPath, "RecipeImages", recipe.FoodImage);
                    if (System.IO.File.Exists(Imgpath))
                    {
                        System.IO.File.Delete(Imgpath);
                    }
                    recipe.FoodImage = UploadFile(recipe.ImageFile);
                }
                db.Update(recipe);
                await db.SaveChangesAsync();
                return RedirectToAction("Recipes");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                return View();
            }
        }
        public async Task<IActionResult> DeleteRecipe(int id)
        {
            try
            {
                var rowdetails = await db.tblRecipes.FirstOrDefaultAsync(p => p.RecipeID == id);
                db.Remove(rowdetails);
                await db.SaveChangesAsync();
                return RedirectToAction("Recipes");
            }
            catch (Exception ex)
            {
                ViewBag.Exception = ex.Message;
                return View();
            }
        }
        public async Task<IActionResult> Details(int id)
        {
            var rowdetails = await db.tblRecipes.FirstOrDefaultAsync(p => p.RecipeID == id);
            string[] Ingredients;
            Ingredients = rowdetails.Ingredients.Split(',');
            if (rowdetails.Ingredients.Contains("\n"))
            {
                Ingredients = rowdetails.Ingredients.Split('\n');
            }
            var Ingredient1 = Ingredients.Take(Ingredients.Length / 2).ToList();
            var Ingredient2 = Ingredients.Skip(Ingredients.Length / 2).ToList();
            ViewBag.Ingredient1 = Ingredient1;
            ViewBag.Ingredient2 = Ingredient2;
            ViewBag.Instructions = rowdetails.Instructions.TrimEnd('.').Split('.');
            if (rowdetails.Instructions.Contains("\n"))
            {
                ViewBag.Instructions = rowdetails.Instructions.Split('\n');
            }
            ViewBag.Tags = rowdetails.Tags.Split(",");
            return View(rowdetails);
        }
        string UploadFile(IFormFile obj)
        {
            string fileName = "";
            if (obj != null && obj.ContentType.ToLower().StartsWith("image"))
            {
                string uploadDir = Path.Combine(env.WebRootPath, "RecipeImages");
                fileName = Guid.NewGuid().ToString() + "_" + obj.FileName;
                string path = Path.Combine(uploadDir, fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    obj.CopyTo(fileStream);
                }
                return fileName;
            }
            return fileName;
        }
    }
}
