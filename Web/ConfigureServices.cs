using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.DataAccess.Database;
using Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Web;

public static class ServiceConfigurator
{
    public static void ConfigureServices(IServiceCollection services)
    {
        const string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";
        
        services.AddControllers();

        services.AddScoped<IAnnotationService, AnnotationService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IThreadService, ThreadService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IAnnotationRepository, AnnotationRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ISearchRepository, SearchDataRepository>();
        services.AddScoped<IUserRepository, AppUserRepository>();
        services.AddScoped<ISearchHistoryRepository, SearchHistoryRepository>();
        services.AddScoped<ISharedRepository, SharedRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddDbContext<DatabaseContext>(options =>
        {
            options
                .UseLoggerFactory(DatabaseContext.MyLoggerFactory)
                .UseNpgsql(database);
        });
    }
}