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
        public DbSet<Answers> Answers { get; set; }
        public DbSet<Search> Search { get; set; }
        public DbSet<PostsTable> PostsTable { get; set; }
        public DbSet<WordRank> WordRank { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string database = HostDatabase.address;

            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .UseNpgsql(database);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CreateMap("Id", "Name");

            modelBuilder.Entity<Search>().HasNoKey();
            modelBuilder.Entity<WordRank>().HasNoKey();
            modelBuilder.Entity<PostsTable>().HasNoKey();

            // modelBuilder.Entity<Answers>().ToTable("answers");
            // modelBuilder.Entity<Answers>().Property(x => x.Parentid).HasColumnName("parentid");

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
