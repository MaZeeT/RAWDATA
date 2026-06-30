using System;
using Application;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.DataAccess.Database;
using Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Web.DependencyRegistration;

public static class ServiceConfigurator
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var database = configuration.GetConnectionString("DefaultConnection")
                       ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        services.AddControllers();

        services.AddScoped<IAnnotationService, AnnotationService>();
        services.AddScoped<IBookmarkService, BookmarkService>();
        services.AddScoped<IHistoryService, HistoryService>();
        services.AddScoped<IThreadService, ThreadService>();
        services.AddScoped<ISearchService, SearchService>();

        services.AddScoped<IAnnotationRepository, AnnotationRepository>();
        services.AddScoped<IHistoryRepository, HistoryRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();
        services.AddScoped<ISearchRepository, SearchDataRepository>();
        services.AddScoped<IUserRepository, AppUserRepository>();
        services.AddScoped<ISearchHistoryRepository, SearchHistoryRepository>();
        services.AddScoped<ISharedRepository, SharedRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddSingleton(new AuthSettings { PasswordSize = configuration.GetValue<int>("Auth:PwdSize") });
        
        services.AddDbContext<DatabaseContext>(options =>
        {
            options
                .UseLoggerFactory(DatabaseContext.MyLoggerFactory)
                .UseNpgsql(database);
        });
    }
}