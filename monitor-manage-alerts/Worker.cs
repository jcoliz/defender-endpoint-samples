using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Azure.Identity;
using MdeSamples.Data;
using MdeSamples.Models;
using MdeSamples.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace MdeSamples;

public class Worker(ILogger<Worker> logger, GraphServiceClient graphClient, IMapper mapper, IAlertStorage alertStorage) : BackgroundService
{
    private readonly JsonSerializerOptions _jsonoptions = new() 
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault 
    };

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

                    // And dump them, for testing
                    foreach(var alert in alerts)
                    {
                        logger.LogInformation("Alert: {alert}", JsonSerializer.Serialize(alert, _jsonoptions));
                    }

                    // Add them to storage
                    await alertStorage.AddRangeAsync(alerts);
                }

                // TODO: Process update tasks here
                // var updates = UpdateFeature.NewUpdates();
                // AlertFeature.SendUpdate(update)
                // UpdateFeature.MarkAsSet(update)

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
