using Microsoft.EntityFrameworkCore;
using MdeSamples.Models;

namespace MdeSamples.Data;

/// <summary>
/// Entity Framework storage for application data
/// </summary>
public class ApplicationDbContext: DbContext
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <remarks>
    /// Needed for creating migrations
    /// </remarks>
    public ApplicationDbContext()
    {

    }

    ///<inheritdoc/>
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
        
    }

    /// <summary>
    /// The alerts we are storing
    /// </summary>
    public DbSet<Alert> Alerts { get; set; }

    /// <summary>
    /// The update tasks we are storing
    /// </summary>
    public DbSet<UpdateAlertTask> UpdateAlertTasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Alert>()
            .HasIndex(x => x.AlertId)
            .IsUnique();

        builder.Entity<UpdateAlertTask>()
            .HasIndex(x => x.Status);

        base.OnModelCreating(builder);
    }
}
