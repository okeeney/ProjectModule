using Microsoft.EntityFrameworkCore;


namespace ImageTextExtraction.Models
{
    public class AppDbContext: DbContext 
    {
        protected readonly IConfiguration Configuration;

        public AppDbContext() { }

        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
        }

        public virtual DbSet<UserDocument> Documents { get; set; }
    }
}
