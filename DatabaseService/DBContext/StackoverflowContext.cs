using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DatabaseService
{
    static class ModelBuilderExtensions
    {
        /// <summary>
        /// Method from class example that converts the names of the tables into lovercases 
        /// Needed for automapper and making life easier
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="names"></param>
        public static void CreateMap(this ModelBuilder modelBuilder, params string[] names)
        {
            //getting access to the entity types dbSet below
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // getting the name of Categories and Products to lower cases.
                entityType.SetTableName(entityType.GetTableName().ToLower());
                foreach (var property in entityType.GetProperties())
                {
                    var propertyName = property.Name.ToLower();
                    /*  //.when names are = to property then i want to extract and put that in lowercase
                      var entityName = "";
                      if (names.ToList().Contains(property.Name)) // names is converted here into a list of strings not array anymore
                      {
                          entityName = entityType.ClrType.Name.ToLower(); // this also gets rid of all stuff including id and name (here we add the name of the class in front of the attribute: categoryname)
                      }*/

                    property.SetColumnName(propertyName);
                }
            }
        }
    }


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

        public DbSet<Annotations> Annotations { get; set; }
        //public DbQuery<AnnotationFunction> AnnotationFunction { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string database = "";
            //database = "host=localhost;db=stackoverflow;uid=postgres;pwd=cock";
            database = "host=mazeet.ddns.net;port=32999;db=stackoverflow;uid=raw6;pwd=J8cxYN";

            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .UseNpgsql(database);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CreateMap("Id", "Name");
            modelBuilder.Entity<Search>().HasNoKey(); //can maybe be hadnled with hasnokey()
            modelBuilder.Entity<WordRank>().HasNoKey();


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