// Copyright (C) 2024 James Coliz, Jr. <jcoliz@outlook.com> All rights reserved
// Use of this source code is governed by the MIT license (see LICENSE.md)

using Azure.Messaging.EventHubs.Consumer;
using HelloWorld.Options;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

try
{
    //
    // Load configuration
    //

    var configuration = new ConfigurationBuilder()
        .AddTomlFile("config.toml", optional: false, reloadOnChange: true)
        .Build();

    AppOptions options = new();
    configuration.Bind(AppOptions.Section, options);

    //
    // Dump the configuration to make sure it's working
    //

    var jsonoptions = new JsonSerializerOptions() 
    { 
        WriteIndented = true, 
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault 
    };

    Console.WriteLine("CONFIG: {0}", JsonSerializer.Serialize(options, jsonoptions));

    //
    // Create the Event Hub Consumer
    //
    // See https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs/README.md#read-events-from-an-event-hub
    //

    string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

    await using (var consumer = new EventHubConsumerClient(consumerGroup, options.EventHub!.ConnectionString, options.EventHub.HubName))
    {
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.CancelAfter(TimeSpan.FromSeconds(45));

        await foreach (PartitionEvent receivedEvent in consumer.ReadEventsAsync(cancellationSource.Token))
        {
            // At this point, the loop will wait for events to be available in the Event Hub.  When an event
            // is available, the loop will iterate with the event that was received.  Because we did not
            // specify a maximum wait time, the loop will wait forever unless cancellation is requested using
            // the cancellation token.

            var raw = receivedEvent.Data.EventBody.ToString();
            using var jDoc = JsonDocument.Parse(raw);
    
            Console.WriteLine("EVENT: {0}", JsonSerializer.Serialize(jDoc, jsonoptions));
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine("ERROR: {0} {1}", ex.GetType().Name, ex.Message);
}
