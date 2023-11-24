﻿using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseService
{
    public class DatabaseContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory
            = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            }); //This is taken from online documentation when we want to log errors

        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<Annotations> Annotations { get; set; }
        public DbSet<AnnotateFunctionDto> AnnotateFunction { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Answers> Answers { get; set; }
        public DbSet<Searches> Searches { get; set; }
        public DbSet<Search> Search { get; set; }
        public DbSet<PostsTable> PostsTable { get; set; }
        public DbSet<WordRank> WordRank { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";

            optionsBuilder
                .UseLoggerFactory(MyLoggerFactory)
                .UseNpgsql(database);
        }       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.CreateMap("Id", "Name");
            modelBuilder.CreateMap();

            modelBuilder.Entity<Search>().HasNoKey();
            modelBuilder.Entity<WordRank>().HasNoKey();
            modelBuilder.Entity<PostsTable>().HasNoKey();
            modelBuilder.Entity<AnnotateFunctionDto>().HasNoKey();
            modelBuilder.Entity<AppUser>().ToTable("appusers");
            modelBuilder.Entity<AppUser>().Property(x => x.Id).HasColumnName("id");

            modelBuilder.Entity<AppUser>(appUser =>
            {
                appUser.ToTable("appusers");
                appUser.Property(x => x.Id).HasColumnName("id");
            });
        }
    }
}