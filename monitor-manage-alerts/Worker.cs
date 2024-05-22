using Azure.Identity;
using HelloWorld.Options;
using Microsoft.Extensions.Options;
using Microsoft.Graph;

namespace monitor_manage_alerts;

public class Worker(ILogger<Worker> logger, IOptions<IdentityOptions> options) : BackgroundService
{
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

            GraphServiceClient graphClient = new GraphServiceClient(clientSecretCredential, options.Value.Scopes);

            logger.LogInformation("Client OK");

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
                    logger.LogInformation("Received {count} alerts", result?.Value?.Count);
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
}
