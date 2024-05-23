using MdeSamples.Data;
using MdeSamples.Models;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Stores alerts in database
/// </summary>
/// <param name="dbContextFactory">Where to get a dbcontext</param>
public class AlertStorage(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IAlertStorage
{
    /// <inheritdoc/>
    public async Task AddRangeAsync(IEnumerable<Alert> alerts)
    {
        using (ApplicationDbContext dbContext = dbContextFactory.CreateDbContext())
        {
            // Collect the alerts we already have, by Id
            var ids = alerts.Select(x => x.AlertId).ToHashSet();
            var duplicates = dbContext.Set<Alert>().Where(x => ids.Contains(x.AlertId)).Select(x=>x.AlertId).ToHashSet();

            // Remove duplicates
            ids.ExceptWith(duplicates);
            
            if (ids.Count > 0)
            {
                // Choose alerts to be added
                var adding = alerts.Where(x => ids.Contains(x.AlertId));

                // Add them
                dbContext.Set<Alert>().AddRange(adding);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
