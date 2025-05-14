using System;
using System.Text;
using BusinessLogic.Implementations;
using BusinessLogic.Interfaces;
using Infrastructure.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Repositories.Implementation;
using Repositories.Interfaces;

string database = "host=localhost;port=5432;db=stackoverflow;uid=postgres;pwd=Password123";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IAnnotationService, AnnotationService>();
builder.Services.AddSingleton<IBookmarkService, BookmarkService>();
builder.Services.AddSingleton<IHistoryService, HistoryService>();
builder.Services.AddSingleton<IUserService, UserService>();

builder.Services.AddSingleton<ISharedRepository, SharedRepositoryRepository>();
builder.Services.AddSingleton<ISearchRepository, SearchRepositoryDataRepository>();
builder.Services.AddSingleton<IAnnotationRepository, AnnotationRepositoryRepository>();
builder.Services.AddSingleton<IUserRepository, AppUserRepositoryRepository>();
builder.Services.AddSingleton<IHistoryRepository, HistoryRepositoryRepository>();
builder.Services.AddSingleton<ISearchHistoryRepository, SearchHistoryRepositoryRepository>();

builder.Services.AddPooledDbContextFactory<DatabaseContext2>(options =>
{
    options
        .UseLoggerFactory(DatabaseContext2.MyLoggerFactory)
        .UseNpgsql(database);
});

var key = Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Auth:Key").Value);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//Needed for serving wwwroot files frontend
app.UseFileServer();

//Needed for api routing
app.UseRouting();

//Authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
