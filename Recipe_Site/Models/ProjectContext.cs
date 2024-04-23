using Microsoft.EntityFrameworkCore;

namespace Recipe_Site.Models
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> option) : base(option)
        {
        }
        public DbSet<Users> tblUsers { get; set; }
        public DbSet<Recipe> tblRecipes { get; set; }

    }
}
