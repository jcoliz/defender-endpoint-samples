using Microsoft.EntityFrameworkCore;

namespace MdeSamples.Data;

/// <summary>
/// Host builder extensions for handling the database
/// </summary>
public static class HostExtensions
{
    /// <summary>
    /// Add the database context (factory, actually)
    /// </summary>
    /// <param name="services">Where to add</param>
    /// <param name="context">Where to get configuration details</param>
    public static void AddDbContext(this IServiceCollection services, HostBuilderContext context)
    {
        var connectionString =
            context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContextFactory<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    /// <summary>
    /// Ensure the database is ready to go
    /// </summary>
    /// <param name="host">Host we are running on</param>
    public static void PrepareDatabase(this IHost host)
    {
        // Migrate the database as needed every run
        // Helpful for samples, but obviously we aren't doing this in production 
        var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
}
