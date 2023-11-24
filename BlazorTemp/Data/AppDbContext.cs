using BlazorTemp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Category> Categories { get; set; } = null!;
    }
}
