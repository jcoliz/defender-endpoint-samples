using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MdeSamples.Data;
using MdeSamples.Models;
using Microsoft.Graph;

namespace MdeSamples;

/// <summary>
/// Perform repeated work
/// </summary>
/// <param name="logger">Where to send logs</param>
/// <param name="graphClient">How to contact Graph service</param>
/// <param name="mapper">How to map objects</param>
/// <param name="alertStorage">Where to store alerts</param>
public class Worker(ILogger<Worker> logger, GraphServiceClient graphClient, IMapper mapper, IAlertStorage alertStorage) : BackgroundService
{
    private readonly JsonSerializerOptions _jsonoptions = new() 
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault 
    };

    ///<inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {        
        try
        {
            logger.LogInformation("Starting");

            //
            // Continually fetch alerts
            //

            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await graphClient.Security.Alerts_v2.GetAsync();

                if (result?.Value is null)
                {
                    logger.LogWarning("Request failed");
                }
                else
                {
                    logger.LogInformation("Received {count} alerts", result.Value.Count);

                    // Map the alerts into local models
                    var alerts = mapper.Map<List<Alert>>(result.Value);

                    // Dump them, for testing
                    foreach(var alert in alerts)
                    {
                        logger.LogInformation("Alert: {alert}", JsonSerializer.Serialize(alert, _jsonoptions));
                    }

                    // Add them to storage
                    var numAdded = await alertStorage.AddRangeAsync(alerts);
                    logger.LogInformation("Added {count} alerts", numAdded);
                }

                // TODO: Process update tasks here
                // var updates = UpdateFeature.NewUpdates();
                // ... Do the update here ...
                // UpdateFeature.MarkAsSent(update)

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            logger.LogInformation("Cancelled");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex,"Failed");
        }
    }
}
