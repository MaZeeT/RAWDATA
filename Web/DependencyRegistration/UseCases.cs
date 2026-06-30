using Application.UseCases.Users.CreateUser;
using Application.UseCases.Users.LoginUser;
using Microsoft.Extensions.DependencyInjection;

namespace Web.DependencyRegistration;

public static class UseCases
{
    public static void Register(IServiceCollection services)
    {
        RegisterUserUseCases(services);
    }

    private static void RegisterUserUseCases(IServiceCollection services)
    {
        services.AddScoped<ICreateUser, CreateUser>();
        services.AddScoped<ILoginUser, LoginUser>();
    }
    
}