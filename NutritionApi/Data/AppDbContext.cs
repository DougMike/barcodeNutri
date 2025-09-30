using Microsoft.EntityFrameworkCore;
using NutritionApi.Models;

namespace NutritionApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }
        public DbSet<Product> Products { get; set; }
    }
}
