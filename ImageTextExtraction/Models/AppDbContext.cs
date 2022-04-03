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
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            string connectionString = "Server=user-documents.cx2fek4ozu91.eu-west-1.rds.amazonaws.com,1433;Database=user-documents;User Id=admin;Password=0151nK**n*y;";
            options.UseSqlServer(connectionString);
        }

        public virtual DbSet<UserDocument> Documents { get; set; }
    }
}
