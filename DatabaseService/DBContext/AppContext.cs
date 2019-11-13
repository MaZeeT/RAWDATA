using DatabaseService.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DatabaseService
{
    public class AppContext : DbContext
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
        public DbSet<Search> Search { get; set; }
        public DbSet<PostsTable> PostsTable { get; set; }
        public DbSet<WordRank> WordRank { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string database = "";
            database = "host=localhost;db=stackoverflow;uid=postgres;pwd=cock";
           // database = "host=localhost;db=stackoverflow;uid=postgres;pwd=Pisi2828";
            //database = "host=mazeet.ddns.net;port=32999;db=stackoverflow;uid=raw6;pwd=J8cxYN";


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
            modelBuilder.Entity<AnnotateFunctionDto>().HasNoKey();
            //modelBuilder.Entity<AnnotationsQuestions>().HasNoKey();

            //modelBuilder.Entity<AuthUsers>().ToTable("authusers");
            //modelBuilder.Entity<AppUser>(); //can maybe be hadnled with hasnokey()
            //modelBuilder.Entity<object /*todo replace type*/>().HasNoKey();


            modelBuilder.Entity<AppUser>().ToTable("appusers");
            modelBuilder.Entity<AppUser>().Property(x => x.Id).HasColumnName("id");

        }
        
    }
}