using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MdeSamples.Data;
using MdeSamples.Models;
using Microsoft.Graph;
using Microsoft.Graph.Models.Security;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using Alert = MdeSamples.Models.Alert;

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
                    logger.LogWarning("Get alerts request failed");
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

                var updates = await alertStorage.GetUpdatesAsync();
                foreach(var update in updates)
                {
                    await PostUpdate(update);
                }

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

    protected async Task PostUpdate(UpdateAlertTask update)
    {
        try
        {
            if (string.IsNullOrEmpty(update.Subject.AlertId))
            {
                logger.LogWarning("Update {id} has no alert.", update.Id);
                return;
            }

            if (update.Action == UpdateAction.Comment)
            {
                var posted = await SetComment(update.Subject.AlertId, update.Payload);

                if (posted is null)
                {
                    throw new ApplicationException("Set ommentfailed");
                }

                logger.LogInformation("Posted comment OK {comments}",JsonSerializer.Serialize(posted, _jsonoptions));

                await alertStorage.MarkAsSentAsync(update);
            }
            else
            {
                var posted = await PatchUpdate(update);

                if (posted is null)
                {
                    throw new ApplicationException("Patch update failed");
                }

                // This returns a whole alert with updated values
                var newAlert = mapper.Map<Alert>(posted);
                await alertStorage.AddOrUpdateAlertAsync(newAlert);

                logger.LogInformation("Posted update OK {result}",JsonSerializer.Serialize(newAlert, _jsonoptions));

                await alertStorage.MarkAsSentAsync(update);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to post update");
        }
    }

    protected Task<List<Microsoft.Graph.Models.Security.AlertComment>?> SetComment(string alertId, string commentText)
    {
        // Currently fails
        // See https://github.com/microsoftgraph/msgraph-sdk-dotnet/issues/2514
#if true
        logger.LogWarning("Adding comments is not working in SDK right now.");
        return Task.FromResult<List<Microsoft.Graph.Models.Security.AlertComment>?>(null);
#else

        Microsoft.Graph.Models.Security.AlertComment comment = new() {
             OdataType = "microsoft.graph.security.alertComment", 
             Comment = commentText
        };
        List<Microsoft.Graph.Models.Security.AlertComment> body = [ comment ];

        return graphClient.Security.Alerts_v2[alertId].Comments.PostAsync(body);
#endif
    }

    protected Task<Microsoft.Graph.Models.Security.Alert?> PatchUpdate(UpdateAlertTask update)
    {
        Microsoft.Graph.Models.Security.Alert body = update.Action switch {
            UpdateAction.AssignedTo => new() { AssignedTo = update.Payload },
            UpdateAction.Classification => new() { Classification = Enum.Parse<AlertClassification>(update.Payload) },
            UpdateAction.Determination => new() { Determination = Enum.Parse<AlertDetermination>(update.Payload) },
            UpdateAction.Status => new() { Status = Enum.Parse<AlertStatus>(update.Payload) },
            _ => throw new NotImplementedException()
        };

        logger.LogInformation("Posting alert update: {alert}", JsonSerializer.Serialize(body, _jsonoptions));

        return graphClient.Security.Alerts_v2[update.Subject.AlertId].PatchAsync(body);
    }
}