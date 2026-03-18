// TODO implement testcontainers -- https://www.youtube.com/watch?v=ssRE0pBNvpE
/* using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Infrastructure.Database;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class IntegrationTestWebApplicationFactory 
    : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder("postgres:latest")
            .WithDatabase("stackoverflow")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Add PostgreSQL from container
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            // Build service provider and run migrations
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.Migrate();
        });
    }

    public async ValueTask InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        ApplyMigrations();
    }

    public async ValueTask DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }

    private async void ApplyMigrations()
    {
        try
        {
            var originalSchema = await File.ReadAllTextAsync("SQL/stackoverflow_create_tabels.sql", TestContext.Current.CancellationToken);
            var schemaChangeMigration = await File.ReadAllTextAsync("SQL/migration-data.sql", TestContext.Current.CancellationToken);

            await _postgresContainer.ExecScriptAsync(originalSchema, TestContext.Current.CancellationToken);
            await _postgresContainer.ExecScriptAsync(schemaChangeMigration, TestContext.Current.CancellationToken);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Applying migrations failed");
            Console.WriteLine(exception);
        }
    }

}*/