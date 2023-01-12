using System;
using System.Text;
using DatabaseService;
using DatabaseService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<ISharedService, SharedService>();
builder.Services.AddSingleton<ISearchDataService, SearchDataService>();
builder.Services.AddSingleton<IAnnotationService, AnnotationService>();
builder.Services.AddSingleton<IAppUserService, AppUserService>();
builder.Services.AddSingleton<IHistoryService, HistoryService>();
builder.Services.AddSingleton<ISearchHistoryService, SearchHistoryService>();

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

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();
