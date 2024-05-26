// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.EventHubs.Consumer;
using HelloWorld.Options;
using Microsoft.Extensions.Options;

namespace event_hub_listener;

public class Worker(ILogger<Worker> logger, IOptions<EventHubOptions> ehubOptions) : BackgroundService
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
            // Create the Event Hub Consumer
            //
            // See https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs/README.md#read-events-from-an-event-hub
            //

            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            await using var consumer = new EventHubConsumerClient(consumerGroup, ehubOptions.Value!.ConnectionString, ehubOptions.Value!.HubName);

            //
            // Get events until cancelled
            //

            await foreach (PartitionEvent receivedEvent in consumer.ReadEventsAsync(stoppingToken))
            {
                // At this point, the loop will wait for events to be available in the Event Hub.  When an event
                // is available, the loop will iterate with the event that was received.  Because we did not
                // specify a maximum wait time, the loop will wait forever unless cancellation is requested using
                // the cancellation token.

                var raw = receivedEvent.Data.EventBody.ToString();
                using var jDoc = JsonDocument.Parse(raw);
        
                // TODO: Consider structured logging for this
                logger.LogInformation("Time: {time} Events: {events}", DateTime.UtcNow.ToUniversalTime(), JsonSerializer.Serialize(jDoc, _jsonoptions));
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
