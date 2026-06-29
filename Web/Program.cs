using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Web.DependencyRegistration;

var builder = WebApplication.CreateBuilder(args);

ServiceConfigurator.ConfigureServices(builder.Services);
UseCases.Register(builder.Services);

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

namespace Web
{
    public partial class Program { }
} // ensures that the custom WebApplicationFactory in the test project can access this class.