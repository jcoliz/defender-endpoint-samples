using Microsoft.EntityFrameworkCore;

namespace MdeSamples.Data;

public static class HostExtensions
{
    public static void AddDbContext(this IServiceCollection services, HostBuilderContext context)
    {
        var connectionString =
            context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }

    public static void PrepareDatabase(this IHost host)
    {
        var scope = host.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Postgres databases are always brought current by the application,
        // because they are only ever used for development and testing.
        //
        // SqlServer databases must be created by EF Core tooling or SQL scripts!

        db.Database.Migrate();
    }
}
