public interface IAlertStorage
{
    /// <summary>
    /// Add the supplied alerts, only if don't already exist by AlertId
    /// </summary>
    /// <param name="alerts"></param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<Alert> alerts);
}