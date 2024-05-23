using MdeSamples.Models;

namespace MdeSamples.Data;

/// <summary>
/// Provides the storage of alerts
/// </summary>
public interface IAlertStorage
{
    /// <summary>
    /// Add the supplied alerts, only if don't already exist by AlertId
    /// </summary>
    /// <param name="alerts"></param>
    /// <returns></returns>
    Task AddRangeAsync(IEnumerable<Alert> alerts);
}