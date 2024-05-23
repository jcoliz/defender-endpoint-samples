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

#if false
            // Just for fun, let's make an alert comment
            var alert = await dbContext.Set<Alert>().Where( x => x.Id == 2).SingleAsync();
            var update  = new UpdateAlertTask() { Subject = alert, Payload = "New comment", Action = UpdateAction.Comment };
            dbContext.Add(update);
            await dbContext.SaveChangesAsync();
#endif
        }

        return result;
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
