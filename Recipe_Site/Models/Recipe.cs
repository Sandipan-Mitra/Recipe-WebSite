using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Recipe_Site.Models
{
    public class Recipe
    {
        [Key]
        public int RecipeID { get; set; }
        public string RecipeDescription { get; set;}
        public string RecipeTitle { get; set;}
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        [DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = @"{0:hh\:mm\:ss}",ApplyFormatInEditMode = true)]
        public DateTime TotalTime { get; set; }
        public string Difficulty { get; set; }
        public string Category { get; set; }
        public string Tags { get; set; }
        public string Author { get; set; }
        public string FoodImage { get; set; }
        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public string Source { get; set; }
        public string Notes { get; set; }
        public string EmailID { get; set; }
    }
}
