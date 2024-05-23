using Microsoft.EntityFrameworkCore;
using MdeSamples.Models;

namespace MdeSamples.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): DbContext(options)
{
    public DbSet<Alert> Alerts { get; set; }

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
