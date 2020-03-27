using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ProjectName.Models
{
  public class ProjectNameContext : IdentityDbContext<ApplicationUser>
  {
    public virtual DbSet<Category> Categories { get; set; }
    public DbSet<Item> Items { get; set; }
    public DbSet<CategoryItem> CategoryItem { get; set; }

    public ProjectNameContext(DbContextOptions options) : base(options) { }
  }
}