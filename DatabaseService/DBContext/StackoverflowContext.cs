using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DatabaseService
{
    public class StackoverflowContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }); //This is taken from online documentation when we want to log errors

        public DbSet<Questions> Questions { get; set; }
        public DbSet<Search> Search { get; set; }
        public DbSet<WordRank> WordRank { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string database = "";
            //database = "host=localhost;db=stackoverflow;uid=postgres;pwd=cock";
            database = "host=localhost;db=stackoverflow;uid=postgres;pwd=Pisi2828"
            //database = "host=mazeet.ddns.net;port=32999;db=stackoverflow;uid=raw6;pwd=J8cxYN";

            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .UseNpgsql(database);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CreateMap("Id", "Name");
            modelBuilder.Entity<Search>().HasNoKey(); //can maybe be hadnled with hasnokey()
            modelBuilder.Entity<WordRank>().HasNoKey();
        //    modelBuilder.Entity<AppUser>().HasNoKey();


            //modelBuilder.Entity<Category>().ToTable("categories");
            //modelBuilder.Entity<Category>().Property(x => x.Id).HasColumnName("categoryid");
            //modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnName("categoryname");
            //modelBuilder.Entity<Category>().Property(x => x.Description).HasColumnName("description");

            //modelBuilder.Entity<Product>().ToTable("products");
            //modelBuilder.Entity<Product>().Property(x => x.Id).HasColumnName("productid");
            //modelBuilder.Entity<Product>().Property(x => x.Name).HasColumnName("productname");
            //modelBuilder.Entity<Product>().Property(x => x.QuantityPerUnit).HasColumnName("quantityperunit");
            //modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unitprice");
            //modelBuilder.Entity<Product>().Property(x => x.UnitsInStock).HasColumnName("unitsinstock");
            //modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnName("categoryid");
        }
    }
}
