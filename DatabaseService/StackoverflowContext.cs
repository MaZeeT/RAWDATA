using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace DatabaseService
{
    static class ModelBuilderExtensions
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void CreateMap(
            this ModelBuilder modelBuilder, 
            params string[] names)
        {
            foreach(var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var dict = new List<string>(names);
                entityType.SetTableName(entityType.GetTableName().ToLower());
                foreach(var property in entityType.GetProperties())
                {
                    var propertyName = property.Name.ToLower();
                    var entityName = "";

                   // if(dict.Contains(property.Name))
                    //{
                    //    entityName = entityType.ClrType.Name.ToLower();
                   // }

                      property.SetColumnName(entityName + propertyName);
                }

            }
        }
    }


    public class StackoverflowContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Search> Search { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                        .UseNpgsql("host=localhost;db=stackoverflow;uid=postgres;pwd=cock");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CreateMap("Id", "Name");
            modelBuilder.Entity<Search>().HasKey(    t => new { t.postid, t.rank });

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
