using DomainServices.Implementations;
using DomainServices.Interfaces;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Implementation;
using Repositories.Interfaces;

namespace WebService;

public static class ServiceConfigurator
{
    public static void ConfigureServices(IServiceCollection services)
    {
        const string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";
        
        services.AddControllers();

        services.AddSingleton<IAnnotationService, AnnotationService>();
        services.AddSingleton<IBookmarkService, BookmarkService>();
        services.AddSingleton<IHistoryService, HistoryService>();
        services.AddSingleton<IThreadService, ThreadService>();
        services.AddSingleton<ISearchService, SearchService>();
        services.AddSingleton<IUserService, UserService>();

        services.AddSingleton<IAnnotationRepository, AnnotationRepository>();
        services.AddSingleton<IHistoryRepository, HistoryRepository>();
        services.AddSingleton<IQuestionRepository, QuestionRepository>();
        services.AddSingleton<ISearchRepository, SearchDataRepository>();
        services.AddSingleton<IUserRepository, AppUserRepository>();
        services.AddSingleton<ISearchHistoryRepository, SearchHistoryRepository>();
        services.AddSingleton<ISharedRepository, SharedRepository>();

        services.AddDbContextFactory<DatabaseContext>(options =>
        {
            options
                .UseLoggerFactory(DatabaseContext.MyLoggerFactory)
                .UseNpgsql(database);
        });
    }
}