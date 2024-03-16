
using Microsoft.EntityFrameworkCore;
using gitaAPI.Controllers;

namespace gitaAPI.Data
{
    public class DbContextClass : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DbContextClass(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("connMSSQL"));
        }

        //public DbSet<Product> Product { get; set; }
        //public DbSet<Company> Company { get; set; }
        //public DbSet<FileDetails> FileDetails { get; set; }
    }


}
