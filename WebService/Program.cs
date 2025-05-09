using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Repositories.Implementation;
using Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IShared, SharedRepository>();
builder.Services.AddSingleton<ISearch, SearchDataRepository>();
builder.Services.AddSingleton<IAnnotation, AnnotationRepository>();
builder.Services.AddSingleton<IUser, AppUserRepository>();
builder.Services.AddSingleton<IHistory, HistoryRepository>();
builder.Services.AddSingleton<ISearchHistory, SearchHistoryRepository>();

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
