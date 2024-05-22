using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using Azure.Identity;
using HelloWorld.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace monitor_manage_alerts;

public class Worker(ILogger<Worker> logger, IOptions<IdentityOptions> options, IMapper mapper) : BackgroundService
{
    private GraphServiceClient? graphClient;

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
            // Setup authentication for Microsoft Graph Service
            //

            if (options.Value is null)
            {
                throw new ApplicationException("Please set Identity options in configuration");
            }

            ClientSecretCredential clientSecretCredential =
                new ClientSecretCredential
                (
                    options.Value.TenantId.ToString(), 
                    options.Value.AppId.ToString(),
                    options.Value.AppSecret
                ); 

            graphClient = new GraphServiceClient(clientSecretCredential, options.Value.Scopes);

            logger.LogInformation("Client OK");

            await InnerLoop(stoppingToken);
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

    private async Task InnerLoop(CancellationToken stoppingToken)
    {
        try
        {
            if (graphClient is null)
            {
                throw new ApplicationException("Client was not initialized");
            }

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

                        // TODO: Insert alerts into database here
                        // AlertFeature.InsertIfNotExists(alert)

                        // TODO: Consider moving graph calls INTO update feature
                    }
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
            throw;
        }
        catch (ApplicationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {messager}", ex.Message);
        }
    }
}
