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
    public async Task<int> AddRangeAsync(IEnumerable<Alert> alerts)
    {
        var result = 0;

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

                result = ids.Count;
            }
        }

        return result;
    }
    public async Task AddOrUpdateAlertAsync(Alert newAlert)
    {
        using (ApplicationDbContext dbContext = dbContextFactory.CreateDbContext())
        {
            // Let's look for the alert first
            var oldAlert = await dbContext.Set<Alert>().Where(x => x.AlertId == newAlert.AlertId).AsNoTracking().SingleOrDefaultAsync();

            if (oldAlert == null)
            {
                dbContext.Add(newAlert);
            }
            else
            {
                dbContext.Update(newAlert with { Id = oldAlert.Id });
            }
            await dbContext.SaveChangesAsync();
        }
    }


    /// <inheritdoc/>
    public async Task<IEnumerable<UpdateAlertTask>> GetUpdatesAsync()
    {
        IEnumerable<UpdateAlertTask> result = Enumerable.Empty<UpdateAlertTask>();

        using (ApplicationDbContext dbContext = dbContextFactory.CreateDbContext())
        {
            result = await dbContext
                .Set<UpdateAlertTask>()
                .Where(x => x.Status == UpdateStatus.New)
                .Include(x=>x.Subject)
                .AsNoTracking()
                .ToArrayAsync();
        }

        return result;
    }

    public async Task MarkAsSentAsync(UpdateAlertTask update)
    {
        using (ApplicationDbContext dbContext = dbContextFactory.CreateDbContext())
        {
            dbContext.Update(update with { Status = UpdateStatus.Sent});
            await dbContext.SaveChangesAsync();
        }        
    }

}
