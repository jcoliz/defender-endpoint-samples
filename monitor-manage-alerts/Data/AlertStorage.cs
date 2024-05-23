using MdeSamples.Data;
using MdeSamples.Models;
using Microsoft.EntityFrameworkCore;

public class AlertStorage(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IAlertStorage
{
    public Task AddRangeAsync(IEnumerable<Alert> alerts)
    {
        throw new NotImplementedException();
    }
}