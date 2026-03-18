using System;
using Domain.AnnotationsDTOs;
using Domain.Models;
using Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Database;

    public class DatabaseContext : DbContext
    {
        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<AppUser> AppUser { get; set; }
        public DbSet<History> History { get; set; }
        public DbSet<Annotations> Annotations { get; set; }
        public DbSet<Questions> Questions { get; set; }
        public DbSet<Answers> Answers { get; set; }
        public DbSet<Searches> Searches { get; set; }
        public DbSet<Search> Search { get; set; }
        public DbSet<PostsTable> PostsTable { get; set; }
        public DbSet<WiWeighted> WiWeighted { get; set; }
        public DbSet<WordRank> WordRank { get; set; }
        public DbSet<QAndA> QAndA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.CreateMap(); // If this is custom, ensure the method exists

            modelBuilder.Entity<Search>().HasNoKey();
            modelBuilder.Entity<Searches>()
                .Property(e => e.SearchType)
                .HasConversion<string>();
            
            modelBuilder.Entity<WordRank>().HasNoKey();
            modelBuilder.Entity<PostsTable>().HasNoKey();

            modelBuilder.Entity<AppUser>().ToTable("appusers");
            modelBuilder.Entity<AppUser>().Property(x => x.Id).HasColumnName("id");
            
            modelBuilder.Entity<WiWeighted>().HasKey(w => new {w.Id, w.What, w.Word});
            
            modelBuilder.Entity<QAndA>().Property(x => x.Id).HasColumnName("id");
            modelBuilder.Entity<QAndA>().ToView("q_and_a");
        }
    
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // Register converter types (these overloads expect a converter type with parameterless ctor)
            configurationBuilder
                .Properties<DateTime>()
                .HaveConversion<UtcDateTimeConverter>();

            configurationBuilder
                .Properties<DateTime?>()
                .HaveConversion<NullableUtcDateTimeConverter>();
        }
}