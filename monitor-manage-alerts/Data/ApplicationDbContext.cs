using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext(): DbContext
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
