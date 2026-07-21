using EventBoard.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventBoard.Api.Tests;

/// <summary>
/// Spins up the real API against an in-memory SQLite database (not the EF InMemory
/// provider). SQLite is used so that raw-SQL features such as the admin events report
/// (ReportsController -> FromSqlRaw) execute exactly as they would in production.
/// The connection is kept open for the lifetime of the factory because an in-memory
/// SQLite database only exists while at least one connection to it is open.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        _connection.Open();

        builder.ConfigureServices(services =>
        {
            // Replace the app's SQLite-file DbContext with our shared in-memory connection.
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // Seed the same reference data the real app starts with.
            DbInitializer.Seed(db);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
