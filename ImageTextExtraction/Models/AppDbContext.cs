using Microsoft.EntityFrameworkCore;

namespace ImageTextExtraction.Models
{
    public class AppDbContext: DbContext 
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<UserDocument> Documents { get; set; }
    }
}
