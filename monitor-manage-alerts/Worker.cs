using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using MdeSamples.Data;
using MdeSamples.Models;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;
using Microsoft.Kiota.Http.HttpClientLibrary;

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
            if (update.Action == UpdateAction.Comment)
            {
                var posted = await SetComment(update.Subject.AlertId, update.Payload);

                if (posted is null)
                {
                    logger.LogWarning("Post alert comment request failed");
                }
                else
                {
                    logger.LogInformation("Posted comment OK {comments}",JsonSerializer.Serialize(posted, _jsonoptions));

                    await alertStorage.MarkAsSentAsync(update);
                }
            }
            else
            {
                logger.LogWarning("Update actions of type {action} are not supported", update.Action.ToString());
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

        Microsoft.Graph.Models.Security.AlertComment comment = new() {
             OdataType = "microsoft.graph.security.alertComment", 
             Comment = commentText
        };
        List<Microsoft.Graph.Models.Security.AlertComment> body = [ comment ];

        return graphClient.Security.Alerts_v2[alertId].Comments.PostAsync(body);
    }
}

public class X : IRequestAdapter
{
    public ISerializationWriterFactory SerializationWriterFactory => throw new NotImplementedException();

    public string? BaseUrl { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public Task<T?> ConvertToNativeRequestAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public void EnableBackingStore(IBackingStoreFactory backingStoreFactory)
    {
        throw new NotImplementedException();
    }

    public Task<ModelType?> SendAsync<ModelType>(RequestInformation requestInfo, ParsableFactory<ModelType> factory, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) where ModelType : IParsable
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ModelType>?> SendCollectionAsync<ModelType>(RequestInformation requestInfo, ParsableFactory<ModelType> factory, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) where ModelType : IParsable
    {
        throw new NotImplementedException();
    }

    public Task SendNoContentAsync(RequestInformation requestInfo, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ModelType?> SendPrimitiveAsync<ModelType>(RequestInformation requestInfo, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ModelType>?> SendPrimitiveCollectionAsync<ModelType>(RequestInformation requestInfo, Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
